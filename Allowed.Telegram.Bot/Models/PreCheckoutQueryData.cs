using Telegram.Bot;
using Telegram.Bot.Types.Payments;

namespace Allowed.Telegram.Bot.Models;

public class PreCheckoutQueryData
{
    public ITelegramBotClient Client { get; set; }
    public PreCheckoutQuery PreCheckoutQuery { get; set; }
    public SimpleTelegramBotClientOptions Options { get; set; }
}