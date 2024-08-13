using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Models;

public class CallbackQueryData
{
    public ITelegramBotClient Client { get; set; }
    public CallbackQuery CallbackQuery { get; set; }
    public SimpleTelegramBotClientOptions Options { get; set; }
}