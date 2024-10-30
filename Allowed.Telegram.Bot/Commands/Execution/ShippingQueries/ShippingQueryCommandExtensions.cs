using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.ShippingQueries;

public static class ShippingQueryCommandExtensions
{
    public static ICommandBuilder MapShippingQueryCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<ShippingQueryCommand>>();
        var command = new ShippingQueryCommand
        {
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<ShippingQueryCommand>(command);
    }

    public static ICommandBuilder MapShippingQueryCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapShippingQueryCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}