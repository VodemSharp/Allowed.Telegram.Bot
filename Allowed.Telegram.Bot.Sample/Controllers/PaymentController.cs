using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Payments;

namespace Allowed.Telegram.Bot.Sample.Controllers;

public class PaymentController : CommandController<int>
{
    [Command("payment")]
    public async Task CreatePayment(MessageData data)
    {
        await data.Client.SendInvoiceAsync(data.Message.From.Id, "Payment", "PaymentDescription",
            Guid.NewGuid().ToString(), "<PaymentToken>", "USD", new List<LabeledPrice>
            {
                new("USD", 100)
            });
    }

    [PreCheckoutQuery]
    public async Task PreCheckout(PreCheckoutQueryData data)
    {
        await data.Client.AnswerPreCheckoutQueryAsync(data.PreCheckoutQuery.Id);
    }
}