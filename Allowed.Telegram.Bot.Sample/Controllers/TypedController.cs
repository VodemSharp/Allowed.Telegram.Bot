namespace Allowed.Telegram.Bot.Sample.Controllers;

// [BotName("Sample")]
// public class TypedController : CommandController<int>
// {
//     [TypedCommand(MessageType.Photo)]
//     public async Task Photo(MessageData data)
//     {
//         await data.Client.SendPhotoAsync(data.Message.From!.Id, data.Message.Photo![0].FileId);
//     }
//
//     [TypedCommand(MessageType.Video)]
//     public async Task Video(MessageData data)
//     {
//         await data.Client.SendVideoAsync(data.Message.From!.Id, data.Message.Video!.FileId);
//     }
//
//     [TypedCommand(MessageType.Audio)]
//     public async Task Audio(MessageData data)
//     {
//         await data.Client.SendAudioAsync(data.Message.From!.Id, data.Message.Audio!.FileId);
//     }
//
//     [TypedCommand(MessageType.ChatMembersAdded)]
//     public async Task ChatMembersAdded(MessageData data)
//     {
//         await data.Client.SendAudioAsync(data.Message.From!.Id, "Chat member added!");
//     }
// }