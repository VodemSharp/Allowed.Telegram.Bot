using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.ChosenInlineResults;

public static class ChosenInlineResultCommandExtensions
{
    public static ICommandBuilder MapChosenInlineResultCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<ChosenInlineResultCommand>>();
        var command = new ChosenInlineResultCommand
        {
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<ChosenInlineResultCommand>(command);
    }

    public static ICommandBuilder MapChosenInlineResultCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapMessageCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}