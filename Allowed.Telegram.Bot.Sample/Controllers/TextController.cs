﻿namespace Allowed.Telegram.Bot.Sample.Controllers;

// [BotName("Sample")]
// public class TextController : CommandController<int>
// {
//     private IUserService<int, ApplicationTgUser> _userService;
//
//     public override void Initialize(IServiceFactory factory, long telegramId)
//     {
//         _userService = factory.CreateUserService<int, ApplicationTgUser>(BotId);
//     }
//
//     [TextCommand]
//     public async Task TextMessage(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id, $"You say: {data.Message.Text}");
//     }
//
//     [TextCommand("Test text command")]
//     public async Task TestTextMessage(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You have selected a test text command!");
//     }
//
//     [TextCommand("Test text command 2")]
//     public async Task TestTextMessage2(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You have selected a test text command 2!");
//     }
//
//     [Command("set_text_test_state")]
//     public async Task SetTest1State(MessageData messageData)
//     {
//         await _userService.SetState(messageData.Message.From!.Id, "TextTestState");
//         await messageData.Client.SendTextMessageAsync(messageData.Message.From!.Id, "Text test state setted!");
//     }
//
//     [TextCommand]
//     [State("TextTestState")]
//     public async Task TestState(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You call text test state method!");
//     }
//
//     [TextCommand("Test state command")]
//     [State("TextTestState")]
//     public async Task TestTextMessageState(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id,
//             "You call text test state method with selected text!");
//     }
//     
//     [TextCommand("Test parameterized command", Type = ComparisonTypes.Parameterized)]
//     [State("TextTestState")]
//     public async Task TestTextMessageStateParameterized(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id,
//             "You call text test state parameterized method with selected text!");
//     }
// }