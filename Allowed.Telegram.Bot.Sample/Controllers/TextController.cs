using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Data.DbModels.Allowed;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.StateServices;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    public class TextController : CommandController
    {
        private readonly IStateService<ApplicationTgState> _stateService;

        public TextController(IStateService<ApplicationTgState> stateService)
        {
            _stateService = stateService;
        }

        [TextCommand]
        public async Task TextMessage(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }

        [Command("set_text_test_state")]
        public async Task SetTest1State(MessageData messageData)
        {
            _stateService.SetState(messageData.Message.Chat.Id, "TextTestState");
            await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "Text test state setted!");
        }

        [TextCommand]
        [State("TextTestState")]
        public async Task TestState(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You call text test state method");
        }
    }
}
