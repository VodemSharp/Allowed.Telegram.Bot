using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace Allowed.Telegram.Bot.Commands.Execution.PreCheckoutQueries;

public class PreCheckoutQueryCommandHandler(
    IServiceProvider provider,
    ICommandCollection<PreCheckoutQueryCommand> commandCollection)
    : CommandHandler<PreCheckoutQueryCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<PreCheckoutQueryCommand> commands, CancellationToken token)
    {
        return Task.FromResult(commands.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        PreCheckoutQueryCommand command, CancellationToken token)
    {
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(PreCheckoutQuery), _ => update.PreCheckoutQuery! }
            }));
    }
}