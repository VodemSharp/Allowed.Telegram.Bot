using Allowed.Telegram.Bot.Clients.Options;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Handlers;

public class TelegramHandler
{
    public ITelegramBotClient Client { get; init; } = null!;
    public SimpleTelegramBotClientOptions Options { get; init; } = null!;
    public CancellationTokenSource? CancellationTokenSource { get; init; }
}