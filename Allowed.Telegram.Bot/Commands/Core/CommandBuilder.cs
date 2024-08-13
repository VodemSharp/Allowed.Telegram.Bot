using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Filters;

namespace Allowed.Telegram.Bot.Commands.Core;

public class CommandBuilder<TCommand>(TCommand command) : ICommandBuilder
    where TCommand : Command
{
    public ICommandBuilder AddFilter<THandler>(params object[] args) where THandler : CommandFilterHandler
    {
        command.Filters.Add(new CommandFilter { Handler = typeof(THandler), Args = args });
        return this;
    }

    public ICommandBuilder AddFilter(CommandFilter filter)
    {
        command.Filters.Add(filter);
        return this;
    }

    public ICommandBuilder AddActionBefore<THandler>() where THandler : CommandActionHandler
    {
        command.ActionsBefore.Add(new CommandAction { Handler = typeof(THandler) });
        return this;
    }

    public ICommandBuilder AddActionAfter<THandler>() where THandler : CommandActionHandler
    {
        command.ActionsAfter.Add(new CommandAction { Handler = typeof(THandler) });
        return this;
    }

    public ICommandBuilder AddActionBefore(CommandAction action)
    {
        command.ActionsBefore.Add(action);
        return this;
    }

    public ICommandBuilder AddActionAfter(CommandAction action)
    {
        command.ActionsAfter.Add(action);
        return this;
    }
}