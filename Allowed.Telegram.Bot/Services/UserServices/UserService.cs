using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Allowed.Telegram.Bot.Services.UserServices
{
    public class UserService<TUser> : IUserService<TUser>
        where TUser : class
    {
        private readonly DbContext _db;

        public UserService(IServiceProvider provider, ContextOptions options)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
        }

        public void CheckUser(long chatId, string username)
        {
            if (GetUser(chatId) == null)
            {
                _db.Add(ContextBuilder.CreateTelegramUser(typeof(TUser), chatId, username));
                _db.SaveChanges();
            }
        }

        public TUser GetUser(long chatId)
        {
            return _db.Set<TUser>().FromSqlRaw(
                "SELECT t.* " +
                "FROM TelegramUsers AS t " +
                "WHERE t.ChatId = {0} " +
                "LIMIT 1", chatId).FirstOrDefault();
        }

        //public void AddUserRole(long chatId, int roleId)
        //{
        //    TUser user = GetUser(chatId);

        //    if (user != null)
        //    {
        //        _db.TelegramUserRoles.Add(new TelegramUserRole<TKey>
        //        {
        //            TelegramUserId = user.Id,
        //            TelegramRoleId = roleId
        //        } as TUserRole);
        //        _db.SaveChanges();
        //    }
        //}


        //public void AddUserRole(long chatId, string role)
        //{
        //    TUser user = GetUser(chatId);
        //    TRole roleEntity = GetRole(role);

        //    if (user != null && roleEntity != null)
        //    {
        //        _db.TelegramUserRoles.Add(new TelegramUserRole<TKey>
        //        {
        //            TelegramUserId = user.Id,
        //            TelegramRoleId = roleEntity.Id
        //        } as TUserRole);
        //        _db.SaveChanges();
        //    }
        //}

        //public IEnumerable<TUser> GetUsersByRole(string role)
        //{
        //    return (IEnumerable<TUser>)_db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
        //        .Where(tur => tur.TelegramRole.Name == role).Select(tur => tur.TelegramUser);
        //}

        //public IEnumerable<TUser> GetUsersByRoles(IEnumerable<string> roles)
        //{
        //    return (IEnumerable<TUser>)_db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
        //        .Where(tur => roles.Contains(tur.TelegramRole.Name)).Select(tur => tur.TelegramUser);
        //}

        //public IEnumerable<TUser> GetUsersByRole(int roleId)
        //{
        //    return (IEnumerable<TUser>)_db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
        //        .Where(tur => tur.TelegramRole.Id == roleId).Select(tur => tur.TelegramUser);
        //}

        //public IEnumerable<TUser> GetUsersByRoles(IEnumerable<int> roleIds)
        //{
        //    return (IEnumerable<TUser>)_db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
        //        .Where(tur => roleIds.Contains(tur.TelegramRole.Id)).Select(tur => tur.TelegramUser);
        //}


        //public bool AnyUserRole(long chatId, int roleId)
        //{
        //    return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
        //        .Any(r => r.TelegramUser.ChatId == chatId && r.TelegramRoleId == roleId);
        //}

        //public bool AnyUserRole(long chatId, string role)
        //{
        //    return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
        //        .Any(r => r.TelegramUser.ChatId == chatId && r.TelegramRole.Name == role);
        //}

        //private TUserRole GetUserRole(long chatId, int roleId)
        //{
        //    return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
        //        .FirstOrDefault(r => r.TelegramUser.ChatId == chatId && r.TelegramRoleId == roleId);
        //}

        //private TUserRole GetUserRole(long chatId, string role)
        //{
        //    return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
        //        .FirstOrDefault(r => r.TelegramUser.ChatId == chatId && r.TelegramRole.Name == role);
        //}

        //public void RemoveUserRole(long chatId, int roleId)
        //{
        //    _db.TelegramUserRoles.Remove(GetUserRole(chatId, roleId));
        //    _db.SaveChanges();
        //}

        //public void RemoveUserRole(long chatId, string role)
        //{
        //    _db.TelegramUserRoles.Remove(GetUserRole(chatId, role));
        //    _db.SaveChanges();
        //}
    }
}
