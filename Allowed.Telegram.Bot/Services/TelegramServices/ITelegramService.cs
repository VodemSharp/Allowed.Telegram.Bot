using Allowed.Telegram.Bot.Models.Store;
using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Services.TelegramServices
{
    public interface ITelegramService
    {
        #region Users

        TelegramUser GetUser(long chatId);
        void CheckUser(long chatId, string username);

        #endregion

        #region States

        TelegramState GetState(long chatId, string botName = "");
        void SetState(long chatId, string value, string botName = "");

        #endregion

        #region Roles

        IEnumerable<TelegramUser> GetUsersByRole(string role);
        IEnumerable<TelegramUser> GetUsersByRole(int roleId);
        IEnumerable<TelegramUser> GetUsersByRoles(IEnumerable<string> roles);
        IEnumerable<TelegramUser> GetUsersByRoles(IEnumerable<int> roleIds);
        IEnumerable<TelegramRole> GetRoles(long chatId);
        TelegramRole GetRole(int roleId);
        TelegramRole GetRole(string role);
        void AddRole(string role);
        void RemoveRole(string role);
        void RemoveRole(int roleId);
        void AddUserRole(long chatId, int roleId);
        void AddUserRole(long chatId, string role);

        #endregion
    }
}
