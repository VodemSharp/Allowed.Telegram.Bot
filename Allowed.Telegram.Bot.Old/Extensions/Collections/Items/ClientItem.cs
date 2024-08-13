using Allowed.Telegram.Bot.Models;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Extensions.Collections.Items;

public class ClientItem
{
    public ITelegramBotClient Client { get; set; }
    public SimpleTelegramBotClientOptions Options { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }
}