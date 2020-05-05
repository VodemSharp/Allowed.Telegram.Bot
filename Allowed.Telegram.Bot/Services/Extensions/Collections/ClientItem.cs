using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.Extensions.Collections
{
    public class ClientItem
    {
        public ITelegramBotClient Client { get; set; }
        public string Name { get; set; }
    }
}
