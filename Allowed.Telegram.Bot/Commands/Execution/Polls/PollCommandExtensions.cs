using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.Polls;

public static class PollCommandExtensions
{
    public static ICommandBuilder MapPollCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<PollCommand>>();
        var command = new PollCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<PollCommand>(command);
    }
    
    public static ICommandBuilder MapPollCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapPollCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}