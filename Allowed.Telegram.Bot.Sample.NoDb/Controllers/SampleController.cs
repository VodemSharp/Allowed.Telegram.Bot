using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.NoDb.Controllers
{
    [BotName("Sample")]
    public class SampleController : CommandController
    {
        [Command("start")]
        public async Task Start(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed: /start");
        }

        [DefaultCommand]
        public async Task DefaultCommand(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed unknown command: {data.Message.Text}");
        }

        [TextCommand]
        public async Task TextCommand(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }
    }
}
