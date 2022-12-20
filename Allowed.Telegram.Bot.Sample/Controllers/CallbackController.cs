using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Allowed.Telegram.Bot.Sample.Controllers;

[BotName("Sample")]
public class CallbackController : CommandController<int>
{
    [Command("query")]
    public async Task CallbackQuery(MessageData data)
    {
        await data.Client.SendTextMessageAsync(
            data.Message.From!.Id,
            "Callback query",
            replyMarkup: new InlineKeyboardMarkup(
                new List<List<InlineKeyboardButton>>
                {
                    new()
                    {
                        new InlineKeyboardButton("True")
                        {
                            CallbackData =
                                new TestCallbackQueryModel
                                {
                                    Path = "test",
                                    SomeData = true
                                }
                        },
                        new InlineKeyboardButton("False")
                        {
                            CallbackData =
                                new TestCallbackQueryModel
                                {
                                    Path = "test",
                                    SomeData = false
                                }
                        },
                        new InlineKeyboardButton("Default")
                        {
                            CallbackData =
                                new CallbackQueryModel
                                {
                                    Path = "default"
                                }
                        }
                    }
                }
            )
        );
    }

    [CallbackQuery("test")]
    public async Task CallbackQuery(CallbackQueryData data, TestCallbackQueryModel model)
    {
        await data.Client.SendTextMessageAsync(data.CallbackQuery.Message!.Chat.Id, $"Model: {model.SomeData}");
        await data.Client.AnswerCallbackQueryAsync(data.CallbackQuery.Id);
    }

    [CallbackDefaultQuery]
    public async Task CallbackDefaultQuery(CallbackQueryData data, CallbackQueryModel model)
    {
        await data.Client.SendTextMessageAsync(data.CallbackQuery.Message!.Chat.Id, "Callback Default Query");
        await data.Client.AnswerCallbackQueryAsync(data.CallbackQuery.Id);
    }
}