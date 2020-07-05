using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    // TextController contains one more good example!
    public class StateController : CommandController
    {
        //private ITelegramService _telegramService { get; set; }
        //
        //public StateController(ITelegramService telegramService)
        //{
        //    _telegramService = telegramService;
        //}

        //[Command("get_state")]
        //public async Task GetState(MessageData messageData)
        //{
        //    await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id,
        //        _telegramService.GetState(messageData.Message.Chat.Id).Value);
        //}
        //
        //[Command("set_state_test1")]
        //public async Task SetTest1State(MessageData messageData)
        //{
        //    _telegramService.SetState(messageData.Message.Chat.Id, "Test1State");
        //    await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "Test1 state setted!");
        //}
        //
        //[Command("set_state_test2")]
        //public async Task SetTest2State(MessageData messageData)
        //{
        //    _telegramService.SetState(messageData.Message.Chat.Id, "Test2State");
        //    await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "Test2 state setted!");
        //}
        //
        //
        //[Command("set_state_test3")]
        //public async Task SetTest3State(MessageData messageData)
        //{
        //    _telegramService.SetState(messageData.Message.Chat.Id, "Test3State");
        //    await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "Test3 state setted!");
        //}

        [State("Test1State")]
        [Command("check_state_test1")]
        public async Task CheckTest1State(MessageData messageData)
        {
            await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "This method allowed for you! (Test1State)");
        }

        [State("Test2State")]
        [Command("check_state_test2")]
        public async Task CheckTest2State(MessageData messageData)
        {
            await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "This method allowed for you! (Test2State)");
        }

        [State("Test1State")]
        [State("Test2State")]
        [Command("check_state_test12")]
        public async Task CheckTest12State(MessageData messageData)
        {
            await messageData.Client.SendTextMessageAsync(messageData.Message.Chat.Id, "This method allowed for you!");
        }
    }
}
