using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    public class TypedController : CommandController
    {
        [TypedCommand(MessageType.Photo)]
        public async Task Photo(MessageData data)
        {
            await data.Client.SendPhotoAsync(data.Message.Chat.Id, data.Message.Photo[0].FileId);
        }

        [TypedCommand(MessageType.Video)]
        public async Task Video(MessageData data)
        {
            await data.Client.SendVideoAsync(data.Message.Chat.Id, data.Message.Video.FileId);
        }

        [TypedCommand(MessageType.Audio)]
        public async Task Audio(MessageData data)
        {
            await data.Client.SendAudioAsync(data.Message.Chat.Id, data.Message.Audio.FileId);
        }
    }
}
