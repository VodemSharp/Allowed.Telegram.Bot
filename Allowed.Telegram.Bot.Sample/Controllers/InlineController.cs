namespace Allowed.Telegram.Bot.Sample.Controllers;

// public class InlineController : CommandController<int>
// {
//     [InlineQuery]
//     public async Task InlineSample(InlineQueryData data)
//     {
//         await data.Client.SendTextMessageAsync(
//             data.InlineQuery.From.Id, $"You enter: {data.InlineQuery.Query}");
//     }
// }