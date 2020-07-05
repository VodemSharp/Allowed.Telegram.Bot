using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    public class TextController : CommandController
    {
        //private readonly ITelegramService _telegramService;
        //
        //public TextController(ITelegramService telegramService)
        //{
        //    _telegramService = telegramService;
        //}

        [TextCommand]
        public async Task TextMessage(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }

        //[Command("set_text_test_state")]
        //public async Task SetTest1State(MessageData messageData)
        //{
        //    _telegramService.SetState(messageData.Message.Chat.Id, "TextTestState");
        //    await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "Text test state setted!");
        //}

        [TextCommand]
        [State("TextTestState")]
        public async Task TestState(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You call text test state method");
        }
    }
}
