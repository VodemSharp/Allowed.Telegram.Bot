using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    public class EmojiController : CommandController
    {
        [EmojiCommand("😀")]
        public async Task EmojiTest(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You start with: 😀");
        }

        [EmojiCommand("🤡")]
        public async Task EmojiClownTest(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You start with: 🤡");
        }

        [EmojiDefaultCommand]
        public async Task EmojiDefault(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You start with default smile");
        }
    }
}
