using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Services.TelegramServices;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class SampleController : CommandController
    {
        private readonly ITelegramService _telegramService;

        public SampleController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        [DefaultCommand]
        public void Default(MessageData data)
        {
            data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }

        [Command("start")]
        public void Start(MessageData data)
        {
            TelegramState message = _telegramService.GetState(data.Message.Chat.Id);

            if (message?.Value == null)
            {
                _telegramService.SetState(data.Message.Chat.Id, "NEW");
                data.Client.SendTextMessageAsync(data.Message.Chat.Id, "You pressed /start. Value = null.");
            }
            else
            {
                data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed /start. Value = {message.Value}.");
                _telegramService.SetState(data.Message.Chat.Id, null);
            }
        }


        [Command("another")]
        public void Another(MessageData data)
        {
            TelegramState message = _telegramService.GetState(data.Message.Chat.Id, "Another");

            if (message?.Value == null)
            {
                _telegramService.SetState(data.Message.Chat.Id, "Another NEW", "Another");
                data.Client.SendTextMessageAsync(data.Message.Chat.Id, "You pressed /start. Value = null.");
            }
            else
            {
                data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed /start. Value = {message.Value}.");
                _telegramService.SetState(data.Message.Chat.Id, null, "Another");
            }
        }

        [TypedCommand(MessageType.Photo)]
        public void Photo(MessageData data)
        {
            data.Client.SendPhotoAsync(data.Message.Chat.Id, data.Message.Photo[0].FileId);
        }

        [TypedCommand(MessageType.Video)]
        public void Video(MessageData data)
        {
            data.Client.SendVideoAsync(data.Message.Chat.Id, data.Message.Video.FileId);
        }

        [TypedCommand(MessageType.Audio)]
        public void Audio(MessageData data)
        {
            data.Client.SendAudioAsync(data.Message.Chat.Id, data.Message.Audio.FileId);
        }
    }
}
