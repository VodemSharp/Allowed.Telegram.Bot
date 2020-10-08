using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class CallbackController : CommandController<int>
    {
        [Command("query")]
        public async Task CallbackQuery(MessageData data)
        {
            await data.Client.SendTextMessageAsync(
                chatId: data.Message.Chat.Id,
                text: $"Callback query",
                replyMarkup: new InlineKeyboardMarkup(
                    new List<List<InlineKeyboardButton>>
                    {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton
                            {
                                Text = "True",
                                CallbackData = JsonConvert.SerializeObject(
                                    new TestCallbackQueryModel
                                    {
                                        Path = "test",
                                        SomeData = true
                                    })
                            },
                            new InlineKeyboardButton
                            {
                                Text = "False",
                                CallbackData = JsonConvert.SerializeObject(
                                    new TestCallbackQueryModel
                                    {
                                        Path = "test",
                                        SomeData = false
                                    })
                            },
                            new InlineKeyboardButton
                            {
                                Text = "Default",
                                CallbackData = JsonConvert.SerializeObject(
                                    new CallbackQueryModel {
                                        Path = "default"
                                    })
                            }
                        }
                    }
                )
            );
        }

        [CallbackQuery("test")]
        public async Task CallbackQuery(CallbackQueryData data, TestCallbackQueryModel model)
        {
            await data.Client.SendTextMessageAsync(data.CallbackQuery.Message.Chat.Id, $"Model: {model.SomeData}");
        }

        [CallbackDefaultQuery]
        public async Task CallbackDefaultQuery(CallbackQueryData data, CallbackQueryModel model)
        {
            await data.Client.SendTextMessageAsync(data.CallbackQuery.Message.Chat.Id, $"Callback Default Query");
        }
    }
}
