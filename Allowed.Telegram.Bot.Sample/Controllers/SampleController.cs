using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class SampleController : CommandController
    {
        [Command("start")]
        public async Task Start(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed: /start");
        }
    }
}
