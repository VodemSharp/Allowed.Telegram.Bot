using Allowed.Telegram.Bot.Clients.Options;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Handlers;

public class TelegramHandler
{
    public ITelegramBotClient Client { get; set; } = null!;
    public SimpleTelegramBotClientOptions Options { get; set; } = null!;
    public CancellationTokenSource? CancellationTokenSource { get; set; }
}