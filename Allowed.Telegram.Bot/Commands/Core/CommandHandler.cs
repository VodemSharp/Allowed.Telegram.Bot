using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Filters;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Core;

public abstract class CommandHandler<TCommand>(
    IServiceProvider provider,
    ICommandCollection<TCommand> commandCollection)
    : ICommandHandler
    where TCommand : Command
{
    protected abstract Task<ExecutableCommand?> GetCommand(
        ITelegramBotClient client, Update update, List<TCommand> commands, CancellationToken token);

    private async Task<List<TCommand>> ApplyFilters(ITelegramBotClient client, Update update, List<TCommand> commands)
    {
        var filters = new List<CommandFilterHandler>();

        // Apply filters
        for (var i = 0; i < commands.Count; i++)
        {
            var badCommand = false;

            foreach (var filter in commands[i].Filters)
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

            if (!badCommand) continue;

            commands.RemoveAt(i);
            i--;
        }

        return commands;
    }

    private async Task InvokeActions(ITelegramBotClient client, Update update, List<CommandAction> actions)
    {
        foreach (var action in actions)
        {
            var handler = (CommandActionHandler)provider.GetRequiredService(action.Handler);
            await handler.Execute(client, update);
        }
    }

    public async Task Invoke(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var globalActions = provider.GetRequiredService<CommandActionGlobalColletion>();
        var commands = await ApplyFilters(client, update, commandCollection.Items);
        var paramCommand = await GetCommand(client, update, commands, token);

        if (paramCommand != null)
        {
            var command = paramCommand.Command;

            await InvokeActions(client, update, globalActions.ActionsBefore);
            await InvokeActions(client, update, command.ActionsBefore);
            command.Handler.Method.Invoke(command.Handler.Target, paramCommand.Parameters.ToArray());
            await InvokeActions(client, update, command.ActionsAfter);
            await InvokeActions(client, update, globalActions.ActionsAfter);
        }
    }
}