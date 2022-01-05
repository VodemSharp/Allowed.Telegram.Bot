using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Models;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class TypedController : CommandController<int>
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

        [TypedCommand(MessageType.ChatMembersAdded)]
        public Task ChatMembersAdded(MessageData data)
        {
            throw new NotImplementedException();
        }
    }
}
