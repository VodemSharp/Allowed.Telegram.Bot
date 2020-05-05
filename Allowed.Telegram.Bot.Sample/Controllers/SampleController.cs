using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class SampleController : CommandController
    {
        [DefaultCommand]
        public void Default(MessageData data)
        {
            data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }

        [Command("start")]
        public void Start(MessageData data)
        {
            data.Client.SendTextMessageAsync(data.Message.Chat.Id, "You pressed /start");
        }
    }
}
