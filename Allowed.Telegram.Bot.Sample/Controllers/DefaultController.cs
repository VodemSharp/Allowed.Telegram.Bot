using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    public class DefaultController : CommandController
    {
        [DefaultCommand]
        public async Task Default(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }
    }
}
