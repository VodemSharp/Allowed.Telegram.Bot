using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Sample.Models;
using Allowed.Telegram.Bot.Services.TelegramServices;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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

        [Command("query")]
        public async Task CallCallbackQuery(MessageData data)
        {
            await data.Client.SendTextMessageAsync(
                chatId: data.Message.Chat.Id,
                text: $"Callback query",
                replyMarkup: new InlineKeyboardMarkup(
                    new InlineKeyboardButton
                    {
                        Text = "Test",
                        CallbackData = JsonConvert.SerializeObject(
                            new TestCallbackQueryModel
                            {
                                Path = "qa",
                                SomeData = true
                            })
                    }
                )
            );
        }

        [CallbackQuery("qa")]
        public void CallbackQuery(CallbackQueryData data, TestCallbackQueryModel model)
        {

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
