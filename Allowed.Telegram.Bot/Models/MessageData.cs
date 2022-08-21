using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Models;

public class MessageData
{
    public ITelegramBotClient Client { get; set; }
    public BotData BotData { get; set; }
    public Message Message { get; set; }
}