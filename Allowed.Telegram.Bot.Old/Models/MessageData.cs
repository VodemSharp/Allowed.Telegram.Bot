using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Models;

public class MessageData
{
    public ITelegramBotClient Client { get; set; }
    public SimpleTelegramBotClientOptions Options { get; set; }
    public Message Message { get; set; }
    public string Params { get; set; }
}