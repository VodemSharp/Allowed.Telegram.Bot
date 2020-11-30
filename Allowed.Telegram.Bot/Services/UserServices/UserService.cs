using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Services.UserServices
{
    public class UserService<TKey, TUser> : IUserService<TKey, TUser>
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
    {
        private readonly ContextOptions _options;
        private readonly DbContext _db;
        private readonly TKey _botId;

        public UserService(IServiceProvider provider, ContextOptions options, TKey botId)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
            _options = options;
            _botId = botId;
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
            else if (!await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t1.* "
                + "FROM TelegramUsers as t1 "
                + "INNER JOIN TelegramBotUsers as t2 ON t1.Id = t2.TelegramUserId "
                + $"WHERE t1.Id = {user.Id} AND t2.TelegramBotId = {_botId} "
                + "LIMIT 1").AnyAsync())
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
            return await _db.Set<TUser>().FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        }

        private async Task<TUser> GetTelegramUser(string username)
        {
            return await _db.Set<TUser>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<TUser>> GetUsers()
        {
            return await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t2.* "
                + "FROM TelegramBotUsers AS t1 "
                + "INNER JOIN TelegramUsers AS t2 ON t1.TelegramUserId = t2.Id "
                + $"WHERE t1.TelegramBotId = {_botId}")
                .ToListAsync();
        }

        public async Task<int> CountUsers()
        {
            return await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t2.* "
                + "FROM TelegramBotUsers AS t1 "
                + "INNER JOIN TelegramUsers AS t2 ON t1.TelegramUserId = t2.Id "
                + $"WHERE t1.TelegramBotId = {_botId}")
                .CountAsync();
        }

        public async Task<bool> AnyUser(long telegramId)
        {
            return await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t2.* "
                + "FROM TelegramBotUsers AS t1 "
                + "INNER JOIN TelegramUsers AS t2 ON t1.TelegramUserId = t2.Id "
                + $"WHERE t1.TelegramBotId = {_botId} AND t2.TelegramId = {telegramId} "
                + "LIMIT 1").AnyAsync();
        }

        public async Task<bool> AnyUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            return await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t2.* "
                + "FROM TelegramBotUsers AS t1 "
                + "INNER JOIN TelegramUsers AS t2 ON t1.TelegramUserId = t2.Id "
                + $"WHERE t1.TelegramBotId = {_botId} AND t2.Username = '{username}' "
                + "LIMIT 1").AnyAsync();
        }


        public async Task<TUser> GetUser(long telegramId)
        {
            return await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t2.* "
                + "FROM TelegramBotUsers AS t1 "
                + "INNER JOIN TelegramUsers AS t2 ON t1.TelegramUserId = t2.Id "
                + $"WHERE t1.TelegramBotId = {_botId} AND t2.TelegramId = {telegramId} "
                + "LIMIT 1").FirstOrDefaultAsync();
        }

        public async Task<TUser> GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            return await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t2.* "
                + "FROM TelegramBotUsers AS t1 "
                + "INNER JOIN TelegramUsers AS t2 ON t1.TelegramUserId = t2.Id "
                + $"WHERE t1.TelegramBotId = {_botId} AND t2.Username = '{username}' "
                + "LIMIT 1").FirstOrDefaultAsync();
        }

        public async Task<TKey> GetBotUserId(long telegramId)
        {
            return await _db.GetBotUserId(_botId, telegramId);
        }

        public async Task<TKey> GetBotUserId(string username)
        {
            return await _db.GetBotUserId(_botId, username);
        }

        public async Task<List<TUser>> GetUsersByRole(string role)
        {
            return await _db.Set<TUser>().FromSqlRaw(
                  "SELECT t4.* "
                + "FROM (((TelegramBotUserRoles as t1 "
                + "INNER JOIN TelegramRoles as t2 ON t1.TelegramRoleId = t2.Id) "
                + "INNER JOIN TelegramBotUsers as t3 ON t1.TelegramBotUserId = t3.Id) "
                + "INNER JOIN TelegramUsers as t4 ON t3.TelegramUserId = t4.Id) "
                + $"WHERE t2.Name = '{role}' AND t3.TelegramBotId = {_botId}").ToListAsync();
        }
    }
}
