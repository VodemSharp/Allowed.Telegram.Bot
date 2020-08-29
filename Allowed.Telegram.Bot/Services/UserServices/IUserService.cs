using Allowed.Telegram.Bot.Models.Store;
using System;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Services.UserServices
{
    public interface IUserService<TKey, TUser>
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
    {
        Task CheckUser(long chatId, string username);
    }
}
