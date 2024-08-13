using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Filters;

namespace Allowed.Telegram.Bot.Commands.Core;

public interface ICommandBuilder
{
    ICommandBuilder AddFilter<THandler>(params object[] args) where THandler : CommandFilterHandler;
    ICommandBuilder AddFilter(CommandFilter filter);

    public ICommandBuilder AddActionBefore<THandler>() where THandler : CommandActionHandler;
    public ICommandBuilder AddActionAfter<THandler>() where THandler : CommandActionHandler;
    
    public ICommandBuilder AddActionBefore(CommandAction action);
    public ICommandBuilder AddActionAfter(CommandAction action);
}