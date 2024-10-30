using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.PollAnswers;

public static class PollAnswerCommandExtensions
{
    public static ICommandBuilder MapPollAnswerCommand(this IHost host, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<PollAnswerCommand>>();
        var command = new PollAnswerCommand
        {
            Handler = handler
        };
        
        collection.Items.Add(command);

        return new CommandBuilder<PollAnswerCommand>(command);
    }
    
    public static ICommandBuilder MapPollAnswerCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapPollAnswerCommand(handler);
        builder.ApplyEffects(command);

        return command;
    }
}