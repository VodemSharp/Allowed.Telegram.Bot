using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.Updates;

public static class UpdateCommandExtensions
{
    public static ICommandBuilder MapUpdateCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<UpdateCommand>>();
        var command = new UpdateCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<UpdateCommand>(command);
    }
    
    public static ICommandBuilder MapUpdateCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapUpdateCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}