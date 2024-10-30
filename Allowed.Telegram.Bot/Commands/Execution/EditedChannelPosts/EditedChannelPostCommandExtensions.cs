using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.EditedChannelPosts;

public static class EditedChannelPostCommandExtensions
{
    public static ICommandBuilder MapEditedChannelPostCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<EditedChannelPostCommand>>();
        var command = new EditedChannelPostCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<EditedChannelPostCommand>(command);
    }
    
    public static ICommandBuilder MapEditedChannelPostCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapEditedChannelPostCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}