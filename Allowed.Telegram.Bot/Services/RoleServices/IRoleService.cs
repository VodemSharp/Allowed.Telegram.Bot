using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Services.RoleServices
{
    public interface IRoleService<TRole>
        where TRole : class
    {
        #region Roles

        List<TRole> GetRoles(long chatId);
        TRole GetRole(int roleId);
        TRole GetRole(string role);

        void AddRole(string role);

        void UpdateRole(string oldRoleName, string newRoleName);
        void UpdateRole(int roleId, string roleName);

        void RemoveRole(string role);
        void RemoveRole(int roleId);

        bool AnyRole(int roleId);
        bool AnyRole(string role);

        #endregion
    }
}
