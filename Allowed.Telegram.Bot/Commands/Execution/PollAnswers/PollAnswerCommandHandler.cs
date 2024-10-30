using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.PollAnswers;

public class PollAnswerCommandHandler(
    IServiceProvider provider,
    ICommandCollection<PollAnswerCommand> commandCollection)
    : CommandHandler<PollAnswerCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<PollAnswerCommand> commands, CancellationToken token)
    {
        return Task.FromResult(commands.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        PollAnswerCommand command, CancellationToken token)
    {
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(PollAnswer), _ => update.PollAnswer! }
            }));
    }
}