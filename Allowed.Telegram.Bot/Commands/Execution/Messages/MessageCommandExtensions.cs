using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.Messages;

public static class MessageCommandExtensions
{
    public static ICommandBuilder MapMessageCommand(this IHost host, string text, Delegate handler,
        MessageCommandTypes type = MessageCommandTypes.Strict)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<MessageCommand>>();
        var command = new MessageCommand
        {
            Text = text,
            Type = type,
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<MessageCommand>(command);
    }

    public static ICommandBuilder MapMessageCommand(this ICommandGroupBuilder builder, string text,
        Delegate handler, MessageCommandTypes type = MessageCommandTypes.Strict)
    {
        var command = builder.Group.Host.MapMessageCommand(text, handler, type);
        builder.ApplyEffects(command);

        return command;
    }

    public static ICommandBuilder MapMessageCommand(this IHost host, Delegate handler)
    {
        return host.MapMessageCommand(string.Empty, handler);
    }

    public static ICommandBuilder MapMessageCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapMessageCommand(string.Empty, handler);
        foreach (var groupFilter in builder.Group.Filters)
            command.AddFilter(groupFilter);

        return command;
    }
}