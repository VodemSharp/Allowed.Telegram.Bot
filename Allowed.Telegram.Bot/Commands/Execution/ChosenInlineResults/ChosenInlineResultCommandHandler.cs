using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.ChosenInlineResults;

public class ChosenInlineResultCommandHandler(
    IServiceProvider provider,
    ICommandCollection<ChosenInlineResultCommand> commandCollection)
    : CommandHandler<ChosenInlineResultCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<ChosenInlineResultCommand> commands, CancellationToken token)
    {
        return Task.FromResult(commands.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        ChosenInlineResultCommand command, CancellationToken token)
    {
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(ChosenInlineResult), _ => update.ChosenInlineResult! }
            }));
    }
}