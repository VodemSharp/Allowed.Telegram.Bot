namespace Allowed.Telegram.Bot.Sample.Controllers;

// public class PaymentController : CommandController<int>
// {
//     [Command("payment")]
//     public async Task CreatePayment(MessageData data)
//     {
//         await data.Client.SendInvoiceAsync(data.Message.From.Id, "Payment", "PaymentDescription",
//             Guid.NewGuid().ToString(), "284685063:TEST:MjFhOTc5NjgzNzFl", "USD", new List<LabeledPrice>
//             {
//                 new("USD", 100)
//             });
//     }
//
//     [PreCheckoutQuery]
//     public async Task PreCheckout(PreCheckoutQueryData data)
//     {
//         await data.Client.AnswerPreCheckoutQueryAsync(data.PreCheckoutQuery.Id);
//     }
// }