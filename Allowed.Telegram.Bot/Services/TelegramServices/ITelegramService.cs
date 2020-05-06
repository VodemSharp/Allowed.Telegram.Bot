using Allowed.Telegram.Bot.Models.Store;

namespace Allowed.Telegram.Bot.Services.TelegramServices
{
    public interface ITelegramService
    {
        TelegramUser GetUser(long chatId);
        void CheckUser(long chatId, string username);
        TelegramState GetState(long chatId, string botName = "");
        void SetState(long chatId, string value, string botName = "");
    }
}
