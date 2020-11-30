using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Data.DbModels.Allowed;
using Allowed.Telegram.Bot.Factories.ServiceFactories;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.StateServices;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class TextController : CommandController<int>
    {
        private IStateService<int, ApplicationTgState> _stateService;

        public override void Initialize(IServiceFactory factory, long telegramId)
        {
            _stateService = factory.CreateStateService<int, ApplicationTgState>(BotId);
        }

        [TextCommand]
        public async Task TextMessage(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }

        [Command("set_text_test_state")]
        public async Task SetTest1State(MessageData messageData)
        {
            await _stateService.SetState(messageData.Message.Chat.Id, "TextTestState");
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
