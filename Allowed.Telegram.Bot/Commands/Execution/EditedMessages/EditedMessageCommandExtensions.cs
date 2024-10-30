using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Commands.Execution.EditedMessages;

public static class EditedMessageCommandExtensions
{
    public static ICommandBuilder MapEditedMessageCommand(
        this IHost host, string text, Delegate handler, MessageType type = MessageType.Text,
        EditedMessageCommandCheckTypes checkType = EditedMessageCommandCheckTypes.Strict)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<EditedMessageCommand>>();
        var command = new EditedMessageCommand
        {
            Text = text,
            Type = type,
            CheckType = checkType,
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<EditedMessageCommand>(command);
    }

    public static ICommandBuilder MapEditedMessageCommand(
        this ICommandGroupBuilder builder, string text, Delegate handler, MessageType type = MessageType.Text,
        EditedMessageCommandCheckTypes checkType = EditedMessageCommandCheckTypes.Strict)
    {
        var command = builder.Group.Host.MapEditedMessageCommand(text, handler, type, checkType);
        builder.ApplyEffects(command);

        return command;
    }

    public static ICommandBuilder MapEditedMessageCommand(
        this IHost host, Delegate handler, MessageType type = MessageType.Text)
    {
        return host.MapEditedMessageCommand(string.Empty, handler, type);
    }

    public static ICommandBuilder MapEditedMessageCommand(
        this ICommandGroupBuilder builder, Delegate handler, MessageType type = MessageType.Text)
    {
        var command = builder.Group.Host.MapEditedMessageCommand(string.Empty, handler, type);
        foreach (var groupFilter in builder.Group.Filters)
            command.AddFilter(groupFilter);

        return command;
    }

    public static ICommandBuilder MapEditedMessageCommand(this IHost host, string text, Delegate handler,
        EditedMessageCommandCheckTypes checkType)
    {
        return host.MapEditedMessageCommand(text, handler, MessageType.Text, checkType);
    }

    public static ICommandBuilder MapEditedMessageCommand(
        this ICommandGroupBuilder builder, string text, Delegate handler, EditedMessageCommandCheckTypes checkType)
    {
        var command = builder.Group.Host.MapEditedMessageCommand(text, handler, MessageType.Text, checkType);
        foreach (var groupFilter in builder.Group.Filters)
            command.AddFilter(groupFilter);

        return command;
    }
}