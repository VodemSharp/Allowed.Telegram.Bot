using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.ChatJoinRequests;

public static class ChatJoinRequestCommandExtensions
{
    public static ICommandBuilder MapChatJoinRequestCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<ChatJoinRequestCommand>>();
        var command = new ChatJoinRequestCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<ChatJoinRequestCommand>(command);
    }
    
    public static ICommandBuilder MapChatJoinRequestCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapChatJoinRequestCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}