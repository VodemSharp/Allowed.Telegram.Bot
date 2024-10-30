using Telegram.Bot;

namespace Allowed.Telegram.Bot.Contexts;

public class TelegramContext
{
    public ITelegramBotClient Client { get; init; } = null!;
    public TelegramBotClientOptions Options { get; init; } = null!;
    public CancellationTokenSource? CancellationTokenSource { get; init; }
}