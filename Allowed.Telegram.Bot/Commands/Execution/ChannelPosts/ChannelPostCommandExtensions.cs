using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.ChannelPosts;

public static class ChannelPostCommandExtensions
{
    public static ICommandBuilder MapChannelPostCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<ChannelPostCommand>>();
        var command = new ChannelPostCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<ChannelPostCommand>(command);
    }
    
    public static ICommandBuilder MapChannelPostCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapChannelPostCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}