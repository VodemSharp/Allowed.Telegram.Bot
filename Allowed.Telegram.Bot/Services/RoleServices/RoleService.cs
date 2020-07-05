using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Allowed.Telegram.Bot.Services.RoleServices
{
    public class RoleService<TRole> : IRoleService<TRole>
        where TRole : class
    {
        private readonly DbContext _db;

        public RoleService(IServiceProvider provider, ContextOptions options)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
        }

        public List<TRole> GetRoles(long chatId)
        {
            return _db.FromSqlRaw<TRole>(
                "SELECT t1.* " +
                "FROM TelegramUserRoles AS t " +
                "INNER JOIN TelegramUsers AS t0 ON t.TelegramUserId = t0.Id " +
                "INNER JOIN TelegramRoles AS t1 ON t.TelegramRoleId = t1.Id " +
               $"WHERE t0.ChatId = {chatId}");
        }

        public TRole GetRole(int roleId)
        {
            return _db.Set<TRole>().FromSqlInterpolated(
                $"SELECT t.Id, t.Name FROM TelegramRoles AS t WHERE t.Id = {roleId} LIMIT 1")
                .FirstOrDefault();
        }

        public TRole GetRole(string roleName)
        {
            return _db.Set<TRole>().FromSqlInterpolated(
                $"SELECT t.Id, t.Name FROM TelegramRoles AS t WHERE t.Name = {roleName} LIMIT 1").FirstOrDefault();
        }

        public void AddRole(string roleName)
        {
            _db.Set<TRole>().Add((TRole)ContextBuilder.CreateTelegramRole(typeof(TRole), roleName));
            _db.SaveChanges();
        }

        private void RemoveRole(TRole role)
        {
            if (role != null)
            {
                _db.Set<TRole>().Remove(role);
                _db.SaveChanges();
            }
        }

        public void RemoveRole(string role)
        {
            RemoveRole(GetRole(role));
        }

        public void RemoveRole(int roleId)
        {
            RemoveRole(GetRole(roleId));
        }

        public void UpdateRole(string oldRoleName, string roleName)
        {
            TRole role = GetRole(oldRoleName);

            if (role != null)
            {
                ReflectionHelper.SetProperty(role, "Name", roleName);
                _db.SaveChanges();
            }
        }

        public void UpdateRole(int roleId, string roleName)
        {
            TRole role = GetRole(roleId);

            if (role != null)
            {
                ReflectionHelper.SetProperty(role, "Name", roleName);
                _db.SaveChanges();
            }
        }

        public bool AnyRole(int roleId)
        {
            return GetRole(roleId) != null;
        }

        public bool AnyRole(string roleName)
        {
            return GetRole(roleName) != null;
        }
    }
}
