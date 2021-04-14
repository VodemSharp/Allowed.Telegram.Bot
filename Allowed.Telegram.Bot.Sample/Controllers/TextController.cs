using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Data.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class TextController : CommandController<int>
    {
        private IUserService<int, ApplicationTgUser> _userService;

        public override void Initialize(IServiceFactory factory, long telegramId)
        {
            _userService = factory.CreateUserService<int, ApplicationTgUser>(BotId);
        }

        [TextCommand]
        public async Task TextMessage(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You say: {data.Message.Text}");
        }

        [Command("set_text_test_state")]
        public async Task SetTest1State(MessageData messageData)
        {
            await _userService.SetState(messageData.Message.Chat.Id, "TextTestState");
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
