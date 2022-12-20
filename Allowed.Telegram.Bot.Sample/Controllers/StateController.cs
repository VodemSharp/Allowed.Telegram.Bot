using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Data.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Sample.Controllers;

[BotName("Sample")]
// TextController contains one more good example!
public class StateController : CommandController<int>
{
    private IUserService<int, ApplicationTgUser> _userService;

    public override void Initialize(IServiceFactory factory, long telegramId)
    {
        _userService = factory.CreateUserService<int, ApplicationTgUser>(BotId);
    }

    [Command("get_state")]
    public async Task GetState(MessageData messageData)
    {
        await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id,
            await _userService.GetState(messageData.Message.From!.Id));
    }

    [Command("set_state_test1")]
    public async Task SetTest1State(MessageData messageData)
    {
        await _userService.SetState(messageData.Message.From!.Id, "Test1State");
        await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id, "Test1 state setted!");
    }

    [Command("set_state_test2")]
    public async Task SetTest2State(MessageData messageData)
    {
        await _userService.SetState(messageData.Message.From!.Id, "Test2State");
        await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id, "Test2 state setted!");
    }

    [Command("set_state_test3")]
    public async Task SetTest3State(MessageData messageData)
    {
        await _userService.SetState(messageData.Message.From!.Id, "Test3State");
        await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id, "Test3 state setted!");
    }

    [State("Test1State")]
    [Command("check_state_test1")]
    public async Task CheckTest1State(MessageData messageData)
    {
        await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id,
            "This method allowed for you! (Test1State)");
    }

    [State("Test2State")]
    [Command("check_state_test2")]
    public async Task CheckTest2State(MessageData messageData)
    {
        await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id,
            "This method allowed for you! (Test2State)");
    }

    [State("Test1State")]
    [State("Test2State")]
    [Command("check_state_test12")]
    public async Task CheckTest12State(MessageData messageData)
    {
        await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id, "This method allowed for you!");
    }
}