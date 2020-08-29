using Allowed.Telegram.Bot.Models.Store;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Services.RoleServices
{
    public interface IRoleService<TKey, TRole>
        where TKey : IEquatable<TKey>
        where TRole : TelegramRole<TKey>
    {
        Task<bool> AnyRole(string role);
        Task<TRole> GetRole(string role);
        Task AddRole(TRole role);
        Task UpdateRole(TRole role);
        Task RemoveRole(TRole role);

        Task<bool> AnyUserRole(long chatId, string role);
        Task AddUserRole(long chatId, string role);
        Task RemoveUserRole(long chatId, string role);

        Task<List<TRole>> GetUserRoles(long chatId);
        Task<List<TRole>> GetUserRoles(string username);
    }
}
