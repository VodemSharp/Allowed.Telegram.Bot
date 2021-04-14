using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.EntityFrameworkCore.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services
{
    public class UserService<TKey, TUser> : IUserService<TKey, TUser>
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
    {
        private readonly LinqHelper _linqHelper;
        private readonly ContextOptions _options;
        private readonly DbContext _db;
        private readonly TKey _botId;

        private readonly IQueryable<TelegramBotUserRole<TKey>> _userRoles;
        private readonly IQueryable<TelegramRole<TKey>> _roles;
        private readonly IQueryable<TelegramUser<TKey>> _users;
        private readonly IQueryable<TelegramBotUser<TKey>> _botUsers;

        public UserService(IServiceProvider provider, ContextOptions options, TKey botId)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
            _options = options;
            _botId = botId;
            _linqHelper = new LinqHelper();

            _userRoles = _db.Set(_options.BotUserType).Cast<TelegramBotUserRole<TKey>>();
            _roles = _db.Set(_options.RoleType).Cast<TelegramRole<TKey>>();
            _users = _db.Set(_options.UserType).Cast<TelegramUser<TKey>>();
            _botUsers = _db.Set(_options.BotUserType).Cast<TelegramBotUser<TKey>>();
        }

        public async Task CheckUser(User telegramUser)
        {
            TUser user = await GetTelegramUser(telegramUser.Id);

            if (user == null && !string.IsNullOrEmpty(telegramUser.Username))
                user = await GetTelegramUser(telegramUser.Username);

            // If user does't use one of bots in db
            if (user == null)
            {
                user = ContextBuilder.CreateTelegramUser<TKey, TUser>(telegramUser.Id, telegramUser.Username,
                    telegramUser.FirstName, telegramUser.LastName, telegramUser.LanguageCode);

                await _db.Set<TUser>().AddAsync(user);
                await _db.SaveChangesAsync();

                await _db.AddAsync(ContextBuilder.CreateTelegramBotUser(_options.BotUserType, user.Id, _botId));
                await _db.SaveChangesAsync();
            }
            // If user exists in db, but doesn't use bot
            else if (!await
                (from u in _users
                 join bu in _botUsers on u.Id equals bu.TelegramUserId
                 where u.Id.Equals(user.Id) && bu.TelegramBotId.Equals(_botId)
                 select u).AnyAsync())
            {
                await _db.AddAsync(ContextBuilder.CreateTelegramBotUser(_options.BotUserType, user.Id, _botId));
                await _db.SaveChangesAsync();
            }

            // If user set username after starting bot
            if (telegramUser.Username != user.Username || telegramUser.Id != user.TelegramId || telegramUser.FirstName != user.FirstName
                || telegramUser.LastName != user.LastName || telegramUser.LanguageCode != user.LanguageCode)
            {
                user.Username = telegramUser.Username;
                user.TelegramId = telegramUser.Id;
                user.FirstName = telegramUser.FirstName;
                user.LastName = telegramUser.LastName;
                user.LanguageCode = telegramUser.LanguageCode;

                _db.Set<TUser>().Update(user);
                await _db.SaveChangesAsync();
            }
        }

        private async Task<TUser> GetTelegramUser(long telegramId)
        {
            return await _db.Set<TUser>().OrderBy(u => u.Id).FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        }

        private async Task<TUser> GetTelegramUser(string username)
        {
            return await _db.Set<TUser>().OrderBy(u => u.Id).FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<TUser>> GetUsers()
        {
            return await (from u in _users
                          join bu in _botUsers on u.Id equals bu.TelegramUserId
                          where bu.TelegramBotId.Equals(_botId)
                          select u).Cast<TUser>().ToListAsync();
        }

        public async Task<int> CountUsers()
        {
            return await (from u in _users
                          join bu in _botUsers on u.Id equals bu.TelegramUserId
                          where bu.TelegramBotId.Equals(_botId)
                          select u).CountAsync();
        }

        public async Task<bool> AnyUser(long telegramId)
        {
            return await (from u in _users
                          join bu in _botUsers on u.Id equals bu.TelegramUserId
                          where bu.TelegramBotId.Equals(_botId) && u.TelegramId == telegramId
                          select u).AnyAsync();
        }

        public async Task<bool> AnyUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            return await (from u in _users
                          join bu in _botUsers on u.Id equals bu.TelegramUserId
                          where bu.TelegramBotId.Equals(_botId) && u.Username == username
                          select u).AnyAsync();
        }


        public async Task<TUser> GetUser(long telegramId)
        {
            return await (from u in _users
                          join bu in _botUsers on u.Id equals bu.TelegramUserId
                          where bu.TelegramBotId.Equals(_botId) && u.TelegramId == telegramId
                          orderby u.Id
                          select u).Cast<TUser>().FirstOrDefaultAsync();
        }

        public async Task<TUser> GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            return await (from u in _users
                          join bu in _botUsers on u.Id equals bu.TelegramUserId
                          where bu.TelegramBotId.Equals(_botId) && u.Username == username
                          orderby u.Id
                          select u).Cast<TUser>().FirstOrDefaultAsync();
        }

        public async Task<TKey> GetBotUserId(long telegramId)
        {
            return await _linqHelper.GetBotUserId(_users, _botUsers, _botId, telegramId);
        }

        public async Task<TKey> GetBotUserId(string username)
        {
            return await _linqHelper.GetBotUserId(_users, _botUsers, _botId, username);
        }

        public async Task<List<TUser>> GetUsersByRole(string role)
        {
            return await (from u in _users
                          join ur in _userRoles on u.Id equals ur.TelegramBotUserId
                          join r in _roles on ur.TelegramRoleId equals r.Id
                          where r.Name == role && ur.TelegramBotUserId.Equals(_botId)
                          select u).Cast<TUser>().ToListAsync();
        }

        private IQueryable<TelegramBotUser<TKey>> GetBotUserQuery(long telegramId)
        {
            return from bu in _botUsers
                   join u in _users on bu.TelegramUserId equals u.Id
                   where bu.TelegramBotId.Equals(_botId) && u.TelegramId == telegramId
                   orderby u.Id
                   select bu;
        }

        private IQueryable<TelegramBotUser<TKey>> GetBotUserQuery(string username)
        {
            return from bu in _botUsers
                   join u in _users on bu.TelegramUserId equals u.Id
                   where bu.TelegramBotId.Equals(_botId) && u.Username == username
                   orderby u.Id
                   select bu;
        }

        public async Task SetState(long telegramId, string value)
        {
            TelegramBotUser<TKey> user = await GetBotUserQuery(telegramId).FirstOrDefaultAsync();
            user.State = value;
            await _db.SaveChangesAsync();
        }

        public async Task SetState(string username, string value)
        {
            TelegramBotUser<TKey> user = await GetBotUserQuery(username).FirstOrDefaultAsync();
            user.State = value;
            await _db.SaveChangesAsync();
        }

        public async Task<string> GetState(long telegramId)
        {
            return await GetBotUserQuery(telegramId).Select(u => u.State).FirstOrDefaultAsync();
        }

        public async Task<string> GetState(string username)
        {
            return await GetBotUserQuery(username).Select(u => u.State).FirstOrDefaultAsync();
        }
    }
}
