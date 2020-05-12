using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Models.Store.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Allowed.Telegram.Bot.Services.TelegramServices
{
    public class TelegramService : ITelegramService
    {
        private AllowedTelegramBotDbContext _db { get; set; }

        public TelegramService(AllowedTelegramBotDbContext db)
        {
            _db = db;
        }

        public void CheckUser(long chatId, string username)
        {
            if (!_db.TelegramUsers.Any(u => u.ChatId == chatId))
            {
                _db.TelegramUsers.Add(new TelegramUser { ChatId = chatId, Username = username });
                _db.SaveChanges();
            }
        }

        public TelegramUser GetUser(long chatId)
        {
            return _db.TelegramUsers.FirstOrDefault(u => u.ChatId == chatId);
        }

        public TelegramBot GetBot(string botName)
        {
            TelegramBot bot = _db.TelegramBots.FirstOrDefault(u => u.Name == botName);

            if (!string.IsNullOrEmpty(botName) && bot == null)
            {
                bot = _db.TelegramBots.Add(new TelegramBot { Name = botName }).Entity;
                _db.SaveChanges();
            }

            return bot;
        }

        public TelegramState GetState(long chatId, string botName = null)
        {
            TelegramUser user = GetUser(chatId);

            if (user == null)
                return null;

            if (string.IsNullOrEmpty(botName))
                return _db.TelegramStates.FirstOrDefault(m => m.TelegramUserId == user.Id);
            else
                return _db.TelegramStates.Include(lm => lm.TelegramBot)
                    .FirstOrDefault(m => m.TelegramUserId == user.Id && m.TelegramBot.Name == botName);
        }

        private TelegramState GetStateByUserId(long userId, string botName = null)
        {
            if (string.IsNullOrEmpty(botName))
                return _db.TelegramStates.FirstOrDefault(m => m.TelegramUserId == userId && m.TelegramBotId == null);
            else
                return _db.TelegramStates.Include(lm => lm.TelegramBot)
                    .FirstOrDefault(m => m.TelegramUserId == userId && m.TelegramBot.Name == botName);
        }

        public void SetState(long chatId, string value, string botName = null)
        {
            TelegramUser telegramUser = GetUser(chatId);
            TelegramBot telegramBot = GetBot(botName);
            TelegramState message = GetStateByUserId(telegramUser.Id, botName);

            if (message == null)
            {
                _db.TelegramStates.Add(new TelegramState
                {
                    TelegramUserId = telegramUser.Id,
                    TelegramBotId = telegramBot?.Id,
                    Value = value
                });
            }
            else
            {
                message.Value = value;
                _db.TelegramStates.Update(message);
            }

            _db.SaveChanges();
        }

        #region Roles

        public IEnumerable<TelegramUser> GetUsersByRole(string role)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
                .Where(tur => tur.TelegramRole.Name == role).Select(tur => tur.TelegramUser);
        }

        public IEnumerable<TelegramUser> GetUsersByRoles(IEnumerable<string> roles)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
                .Where(tur => roles.Contains(tur.TelegramRole.Name)).Select(tur => tur.TelegramUser);
        }

        public IEnumerable<TelegramUser> GetUsersByRole(int roleId)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
                .Where(tur => tur.TelegramRole.Id == roleId).Select(tur => tur.TelegramUser);
        }

        public IEnumerable<TelegramUser> GetUsersByRoles(IEnumerable<int> roleIds)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
                .Where(tur => roleIds.Contains(tur.TelegramRole.Id)).Select(tur => tur.TelegramUser);
        }

        public IEnumerable<TelegramRole> GetRoles(long chatId)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
                .Where(tur => tur.TelegramUser.ChatId == chatId).Select(tur => tur.TelegramRole);
        }

        public TelegramRole GetRole(int roleId)
        {
            return _db.TelegramRoles.FirstOrDefault(r => r.Id == roleId);
        }

        public TelegramRole GetRole(string role)
        {
            return _db.TelegramRoles.FirstOrDefault(r => r.Name == role);
        }

        public void AddRole(string role)
        {
            _db.TelegramRoles.Add(new TelegramRole
            {
                Name = role
            });
            _db.SaveChanges();
        }

        private void RemoveRole(TelegramRole role)
        {
            if (role != null)
            {
                _db.TelegramRoles.Remove(role);
                _db.SaveChanges();
            }
        }

        public void RemoveRole(string role)
        {
            RemoveRole(_db.TelegramRoles.FirstOrDefault(r => r.Name == role));
        }

        public void RemoveRole(int roleId)
        {
            RemoveRole(_db.TelegramRoles.FirstOrDefault(r => r.Id == roleId));
        }

        public void AddUserRole(long chatId, int roleId)
        {
            TelegramUser user = GetUser(chatId);

            if (user != null)
            {
                _db.TelegramUserRoles.Add(new TelegramUserRole
                {
                    TelegramUserId = user.Id,
                    TelegramRoleId = roleId
                });
                _db.SaveChanges();
            }
        }

        public void AddUserRole(long chatId, string role)
        {
            TelegramUser user = GetUser(chatId);
            TelegramRole roleEntity = GetRole(role);

            if (user != null && roleEntity != null)
            {
                _db.TelegramUserRoles.Add(new TelegramUserRole
                {
                    TelegramUserId = user.Id,
                    TelegramRoleId = roleEntity.Id
                });
                _db.SaveChanges();
            }
        }

        public void UpdateRole(string oldRoleName, string newRoleName)
        {
            TelegramRole role = GetRole(oldRoleName);

            if (role != null)
            {
                role.Name = newRoleName;
                _db.SaveChanges();
            }
        }

        public void UpdateRole(int roleId, string roleName)
        {
            TelegramRole role = GetRole(roleId);

            if (role != null)
            {
                role.Name = roleName;
                _db.SaveChanges();
            }
        }

        public bool AnyRole(int roleId)
        {
            return _db.TelegramRoles.Any(r => r.Id == roleId);
        }

        public bool AnyRole(string role)
        {
            return _db.TelegramRoles.Any(r => r.Name == role);
        }

        public bool AnyUserRole(long chatId, int roleId)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
                .Any(r => r.TelegramUser.ChatId == chatId && r.TelegramRoleId == roleId);
        }

        public bool AnyUserRole(long chatId, string role)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
                .Any(r => r.TelegramUser.ChatId == chatId && r.TelegramRole.Name == role);
        }

        private TelegramUserRole GetUserRole(long chatId, int roleId)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
                .FirstOrDefault(r => r.TelegramUser.ChatId == chatId && r.TelegramRoleId == roleId);
        }

        private TelegramUserRole GetUserRole(long chatId, string role)
        {
            return _db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
                .FirstOrDefault(r => r.TelegramUser.ChatId == chatId && r.TelegramRole.Name == role);
        }

        public void RemoveUserRole(long chatId, int roleId)
        {
            _db.TelegramUserRoles.Remove(GetUserRole(chatId, roleId));
            _db.SaveChanges();
        }

        public void RemoveUserRole(long chatId, string role)
        {
            _db.TelegramUserRoles.Remove(GetUserRole(chatId, role));
            _db.SaveChanges();
        }

        #endregion
    }
}
