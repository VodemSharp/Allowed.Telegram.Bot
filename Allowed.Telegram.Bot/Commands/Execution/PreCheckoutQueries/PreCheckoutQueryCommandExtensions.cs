using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.PreCheckoutQueries;

public static class PreCheckoutQueryCommandExtensions
{
    public static ICommandBuilder MapPreCheckoutQueryCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<PreCheckoutQueryCommand>>();
        var command = new PreCheckoutQueryCommand
        {
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<PreCheckoutQueryCommand>(command);
    }

    public static ICommandBuilder MapPreCheckoutQueryCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapPreCheckoutQueryCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}