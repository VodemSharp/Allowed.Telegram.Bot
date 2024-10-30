using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.ChatMembers;

public static class ChatMemberCommandExtensions
{
    public static ICommandBuilder MapChatMemberCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<ChatMemberCommand>>();
        var command = new ChatMemberCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<ChatMemberCommand>(command);
    }
    
    public static ICommandBuilder MapChatMemberCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapChatMemberCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}