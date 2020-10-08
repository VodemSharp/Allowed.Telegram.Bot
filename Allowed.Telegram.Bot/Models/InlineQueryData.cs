using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Models
{
    public class InlineQueryData
    {
        public ITelegramBotClient Client { get; set; }
        public InlineQuery InlineQuery { get; set; }
        public BotData BotData { get; set; }
    }
}
