using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Allowed.Telegram.Bot.Services.UserServices
{
    public class UserService<TUser> : IUserService<TUser>
        where TUser : class
    {
        private readonly DbContext _db;
        private readonly ContextOptions _options;

        public UserService(IServiceProvider provider, ContextOptions options)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
            _options = options;
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

        public void AddUserRole(long chatId, int roleId)
        {
            TUser user = GetUser(chatId);

            if (user != null)
            {
                _db.Set(_options.UserRoleType).Add(
                    ContextBuilder.CreateTelegramUserRole(_options.UserRoleType, user.GetProperty("Id"), roleId));

                _db.SaveChanges();
            }
        }


        public void AddUserRole(long chatId, string roleName)
        {
            TUser user = GetUser(chatId);
            object role = _db.Set(_options.RoleType).FromSqlInterpolated(
                $"SELECT t.* FROM TelegramRoles` AS t WHERE t.Name = '{roleName}' LIMIT 1")
                .FirstOrDefault();

            if (user != null && role != null)
            {
                _db.Set(_options.UserRoleType).Add(ContextBuilder.CreateTelegramUserRole(
                    _options.UserRoleType, user.GetProperty("Id"), role.GetProperty("Id")));
                _db.SaveChanges();
            }
        }

        public IEnumerable<TUser> GetUsersByRole(string roleName)
        {
            return (IEnumerable<TUser>)_db.FromSqlRaw(
                "SELECT t1.* " +
                "FROM TelegramUserRoles AS t " +
                "INNER JOIN TelegramRoles AS t0 ON t.TelegramRoleId = t0.Id " +
                "INNER JOIN TelegramUsers AS t1 ON t.TelegramUserId = t1.Id " +
               $"WHERE t0.Name = '{roleName}'", _options.RoleType).FirstOrDefault();
        }

        public IEnumerable<TUser> GetUsersByRoles(IEnumerable<string> roles)
        {
            return (IEnumerable<TUser>)_db.FromSqlRaw(
                "SELECT t1.* " +
                "FROM TelegramUserRoles AS t " +
                "INNER JOIN TelegramRoles AS t0 ON t.TelegramRoleId = t0.Id " +
                "INNER JOIN TelegramUsers AS t1 ON t.TelegramUserId = t1.Id " +
               $"WHERE t0.Name IN ({string.Join(", ", roles.Select(r => r.Select(ir => $"'{ir}'")))})",
                _options.UserType);
        }

        public IEnumerable<TUser> GetUsersByRole(int roleId)
        {
            return (IEnumerable<TUser>)_db.FromSqlRaw(
                "SELECT t1.* " +
                "FROM TelegramUserRoles AS t " +
                "INNER JOIN TelegramRoles AS t0 ON t.TelegramRoleId = t0.Id " +
                "INNER JOIN TelegramUsers AS t1 ON t.TelegramUserId = t1.Id " +
               $"WHERE t0.Id = '{roleId}'", _options.RoleType).FirstOrDefault();
        }

        public IEnumerable<TUser> GetUsersByRoles(IEnumerable<int> roleIds)
        {
            return (IEnumerable<TUser>)_db.FromSqlRaw(
                "SELECT t1.* " +
                "FROM TelegramUserRoles AS t " +
                "INNER JOIN TelegramRoles AS t0 ON t.TelegramRoleId = t0.Id " +
                "INNER JOIN TelegramUsers AS t1 ON t.TelegramUserId = t1.Id " +
               $"WHERE t0.Name IN ({string.Join(", ", roleIds)})",
                _options.UserType);
        }


        public bool AnyUserRole(long chatId, int roleId)
        {
            return _db.Set(_options.UserRoleType).FromSqlRaw(
                 "SELECT t.* " +
                 "FROM TelegramUserRoles AS t " +
                 "INNER JOIN TelegramUsers AS t0 ON t.TelegramUserId = t0.Id " +
                $"WHERE (t0.ChatId = {chatId}) AND (t.TelegramRoleId = {roleId})" +
                 "LIMIT 1").Any();
        }

        public bool AnyUserRole(long chatId, string roleName)
        {
            return _db.Set(_options.UserRoleType).FromSqlRaw(
                 "SELECT t.* " +
                 "FROM TelegramUserRoles AS t " +
                 "INNER JOIN TelegramUsers AS t0 ON t.TelegramUserId = t0.Id " +
                 "INNER JOIN TelegramRoles AS t1 ON t.TelegramRoleId = t1.Id " +
                $"WHERE (t0.ChatId = {chatId}) AND (t1.Name = {roleName})" +
                 "LIMIT 1").Any();
        }

        private object GetUserRole(long chatId, int roleId)
        {
            return _db.Set(_options.UserRoleType).FromSqlRaw(
                 "SELECT t.* " +
                 "FROM TelegramUserRoles AS t " +
                 "INNER JOIN TelegramUsers AS t0 ON t.TelegramUserId = t0.Id " +
                $"WHERE (t0.ChatId = {chatId}) AND (t.TelegramRoleId = {roleId})" +
                 "LIMIT 1").FirstOrDefault();
        }

        private object GetUserRole(long chatId, string roleName)
        {
            return _db.Set(_options.UserRoleType).FromSqlRaw(
                   "SELECT t.* " +
                   "FROM TelegramUserRoles AS t " +
                   "INNER JOIN TelegramUsers AS t0 ON t.TelegramUserId = t0.Id " +
                   "INNER JOIN TelegramRoles AS t1 ON t.TelegramRoleId = t1.Id " +
                  $"WHERE (t0.ChatId = {chatId}) AND (t1.Name = {roleName})" +
                   "LIMIT 1").FirstOrDefault();
        }

        public void RemoveUserRole(long chatId, int roleId)
        {
            _db.Set(_options.UserRoleType).Remove(GetUserRole(chatId, roleId));
            _db.SaveChanges();
        }

        public void RemoveUserRole(long chatId, string role)
        {
            _db.Set(_options.UserRoleType).Remove(GetUserRole(chatId, role));
            _db.SaveChanges();
        }
    }
}
