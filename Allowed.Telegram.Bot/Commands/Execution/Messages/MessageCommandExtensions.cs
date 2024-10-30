using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Commands.Execution.Messages;

public static class MessageCommandExtensions
{
    public static ICommandBuilder MapMessageCommand(this IHost host, string text, Delegate handler,
        MessageType type = MessageType.Text, MessageCommandCheckTypes checkType = MessageCommandCheckTypes.Strict)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<MessageCommand>>();
        var command = new MessageCommand
        {
            Text = text,
            Type = type,
            CheckType = checkType,
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<MessageCommand>(command);
    }

    public static ICommandBuilder MapMessageCommand(this ICommandGroupBuilder builder, string text, Delegate handler,
        MessageType type = MessageType.Text, MessageCommandCheckTypes checkType = MessageCommandCheckTypes.Strict)
    {
        var command = builder.Group.Host.MapMessageCommand(text, handler, type, checkType);
        builder.ApplyEffects(command);

        return command;
    }

    public static ICommandBuilder MapMessageCommand(
        this IHost host, Delegate handler, MessageType type = MessageType.Text)
    {
        return host.MapMessageCommand(string.Empty, handler, type);
    }

    public static ICommandBuilder MapMessageCommand(
        this ICommandGroupBuilder builder, Delegate handler, MessageType type = MessageType.Text)
    {
        var command = builder.Group.Host.MapMessageCommand(string.Empty, handler, type);
        foreach (var groupFilter in builder.Group.Filters)
            command.AddFilter(groupFilter);

        return command;
    }

    public static ICommandBuilder MapMessageCommand(this IHost host, string text, Delegate handler,
        MessageCommandCheckTypes checkType)
    {
        return host.MapMessageCommand(text, handler, MessageType.Text, checkType);
    }

    public static ICommandBuilder MapMessageCommand(this ICommandGroupBuilder builder, string text, Delegate handler,
        MessageCommandCheckTypes checkType)
    {
        var command = builder.Group.Host.MapMessageCommand(text, handler, MessageType.Text, checkType);
        foreach (var groupFilter in builder.Group.Filters)
            command.AddFilter(groupFilter);

        return command;
    }
}