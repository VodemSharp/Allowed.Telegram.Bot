using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Actions;

public static class CommandActionExtensions
{
    public static IHost AddGlobalActionBefore<TAction>(this IHost host) where TAction : CommandActionHandler
    {
        var globalActions = host.Services.GetRequiredService<CommandActionGlobalCollection>();
        globalActions.ActionsBefore.Add(new CommandAction { Handler = typeof(TAction) });
        return host;
    }

    public static IHost AddGlobalActionAfter<TAction>(this IHost host) where TAction : CommandActionHandler
    {
        var globalActions = host.Services.GetRequiredService<CommandActionGlobalCollection>();
        globalActions.ActionsAfter.Add(new CommandAction { Handler = typeof(TAction) });
        return host;
    }
}