namespace Allowed.Telegram.Bot.Services.UserServices
{
    public interface IUserService<TUser>
        where TUser : class
    {
        #region Users

        TUser GetUser(long chatId);
        void CheckUser(long chatId, string username);

        #endregion

        #region UserRoles

        //IEnumerable<TUser> GetUsersByRole(string role);
        //IEnumerable<TUser> GetUsersByRole(int roleId);
        //IEnumerable<TUser> GetUsersByRoles(IEnumerable<string> roles);
        //IEnumerable<TUser> GetUsersByRoles(IEnumerable<int> roleIds);

        //void AddUserRole(long chatId, int roleId);
        //void AddUserRole(long chatId, string role);

        //void RemoveUserRole(long chatId, int roleId);
        //void RemoveUserRole(long chatId, string role);

        //bool AnyUserRole(long chatId, int roleId);
        //bool AnyUserRole(long chatId, string role);

        #endregion
    }
}
