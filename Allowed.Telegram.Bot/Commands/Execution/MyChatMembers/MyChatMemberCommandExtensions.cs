using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.MyChatMembers;

public static class MyChatMemberCommandExtensions
{
    public static ICommandBuilder MapMyChatMemberCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<MyChatMemberCommand>>();
        var command = new MyChatMemberCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<MyChatMemberCommand>(command);
    }
    
    public static ICommandBuilder MapMyChatMemberCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapMyChatMemberCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}