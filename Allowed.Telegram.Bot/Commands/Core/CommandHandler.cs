using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Attributes;
using Allowed.Telegram.Bot.Commands.Filters;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Core;

public abstract class CommandHandler<TCommand>(
    IServiceProvider provider,
    ICommandCollection<TCommand> commandCollection) : ICommandHandler
    where TCommand : Command
{
    public async Task Invoke(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var globalActions = provider.GetRequiredService<CommandActionGlobalCollection>();
        var tCommands = await ApplyFilters(client, update, commandCollection.Items);
        var commands = await GetCommands(client, update, tCommands, token);
        commands = await CheckAttributes(client, update, commands);

        if (commands.Count != 0)
        {
            var command = commands.Single();
            var parameters = await GetParameters(client, update, (TCommand)command, token);

            await InvokeActions(client, update, globalActions.ActionsBefore);
            await InvokeActions(client, update, command.ActionsBefore);

            var returnType = command.Handler.Method.ReturnType;
            var result = command.Handler.Method.Invoke(command.Handler.Target, parameters.ToArray());

            if (typeof(Task).IsAssignableFrom(returnType))
                await (Task)result!;

            await InvokeActions(client, update, command.ActionsAfter);
            await InvokeActions(client, update, globalActions.ActionsAfter);
        }
    }

    protected abstract Task<List<Command>> GetCommands(ITelegramBotClient client, Update update,
        List<TCommand> commands, CancellationToken token);

    protected abstract Task<List<object?>> GetParameters(ITelegramBotClient client, Update update, TCommand command,
        CancellationToken token);

    private async Task<List<TCommand>> ApplyFilters(ITelegramBotClient client, Update update, List<TCommand> commands)
    {
        var filteredCommands = new List<TCommand>();
        var filters = new List<CommandFilterHandler>();

        foreach (var command in commands)
        {
            var badCommand = false;

            foreach (var filter in command.Filters)
            {
                var createdFilter = filters.SingleOrDefault(x => x.GetType() == filter.Handler);
                if (createdFilter == null)
                {
                    createdFilter = (CommandFilterHandler)provider.GetRequiredService(filter.Handler);
                    await createdFilter.Initialize(client, update);
                    filters.Add(createdFilter);
                }

                if (await createdFilter.Apply(filter.Args)) continue;

                badCommand = true;
                break;
            }

            if (!badCommand)
                filteredCommands.Add(command);
        }

        return filteredCommands;
    }

    private async Task<List<Command>> CheckAttributes(ITelegramBotClient client, Update update, List<Command> commands)
    {
        var handlers = provider.GetRequiredService<CommandAttributeHandlerCollection>().Handlers;
        var appliedCommands = commands.Select(x => x).ToList();

        foreach (var handlerType in handlers)
        {
            var handlerAppliedCommands = new List<Command>();
            CommandAttributeHandler? createdHandler = null;

            for (var i = 0; i < appliedCommands.Count; i++)
            {
                var command = appliedCommands[i];
                var attribute = command.Attributes.SingleOrDefault(a => a.Handler == handlerType);

                if (attribute == null) continue;

                if (createdHandler == null)
                {
                    createdHandler = (CommandAttributeHandler)provider.GetRequiredService(handlerType);
                    await createdHandler.Initialize(client, update);
                }

                if (!await createdHandler.Apply(attribute.Args))
                {
                    appliedCommands.RemoveAt(i);
                    i--;
                }
                else
                {
                    handlerAppliedCommands.Add(command);
                }
            }

            if (handlerAppliedCommands.Count != 0)
                appliedCommands = handlerAppliedCommands;
        }

        return appliedCommands.Count == 0 ? commands : appliedCommands;
    }

    private async Task InvokeActions(ITelegramBotClient client, Update update, List<CommandAction> actions)
    {
        foreach (var action in actions)
        {
            var handler = (CommandActionHandler)provider.GetRequiredService(action.Handler);
            await handler.Execute(client, update);
        }
    }
}