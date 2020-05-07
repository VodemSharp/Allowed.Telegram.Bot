using Allowed.Telegram.Bot.Models;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.Extensions.Collections
{
    public class ClientItem
    {
        public ITelegramBotClient Client { get; set; }
        public BotData BotData { get; set; }
    }
}
