using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.InlineQueries;

public static class InlineQueryCommandExtensions
{
    public static ICommandBuilder MapInlineQueryCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<InlineQueryCommand>>();
        var command = new InlineQueryCommand
        {
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<InlineQueryCommand>(command);
    }

    public static ICommandBuilder MapInlineQueryCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapMessageCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}