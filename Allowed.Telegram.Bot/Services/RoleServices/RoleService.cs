using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Services.RoleServices
{
    public class RoleService<TKey, TRole> : IRoleService<TKey, TRole>
        where TKey : IEquatable<TKey>
        where TRole : TelegramRole<TKey>
    {
        private readonly ContextOptions _options;
        private readonly DbContext _db;
        private readonly TKey _botId;

        public RoleService(IServiceProvider provider, ContextOptions options, TKey botId)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
            _options = options;
            _botId = botId;
        }

        public async Task<bool> AnyRole(string role)
        {
            return await _db.Set<TRole>().AnyAsync(r => r.Name == role);
        }

        public async Task<TRole> GetRole(string role)
        {
            return await _db.Set<TRole>().FirstOrDefaultAsync(r => r.Name == role);
        }

        public async Task AddRole(TRole role)
        {
            await _db.Set<TRole>().AddAsync(role);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateRole(TRole role)
        {
            _db.Set<TRole>().Update(role);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveRole(TRole role)
        {
            _db.Set<TRole>().Remove(role);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> AnyUserRole(long chatId, string role)
        {
            return await _db.Set<TRole>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM (((TelegramRoles as t1 "
              + "INNER JOIN TelegramBotUserRoles as t2 ON t1.Id = t2.TelegramRoleId) "
              + "INNER JOIN TelegramBotUsers as t3 ON t3.Id = t2.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers as t4 ON t4.Id = t3.TelegramUserId) "
              + $"WHERE t4.ChatId = {chatId} AND t3.TelegramBotId = {_botId} AND t1.Name = '{role}' "
              + "LIMIT 1").AnyAsync();
        }

        private async Task<TKey> GetBotUserId(long chatId)
        {
            TKey result;

            var connection = _db.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT t2.Id "
                  + "FROM TelegramUsers as t1 "
                  + "INNER JOIN TelegramBotUsers as t2 ON t1.Id = t2.TelegramUserId "
                  + $"WHERE t1.ChatId = {chatId} AND t2.TelegramBotId = {_botId} "
                  + "LIMIT 1";

                result = (TKey)await command.ExecuteScalarAsync();
            }

            if (connection.State == ConnectionState.Open) await connection.CloseAsync();
            return result;
        }

        public async Task AddUserRole(long chatId, string role)
        {
            TKey botUserId = await GetBotUserId(chatId);
            TKey roleId = (await _db.Set<TRole>().FirstAsync(r => r.Name == role)).Id;

            await _db.AddAsync(ContextBuilder.CreateTelegramBotUserRole(_options.BotUserRoleType, botUserId, roleId));
            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserRole(long chatId, string role)
        {
            var connection = _db.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                        "DELETE t1 " +
                        "FROM (((TelegramBotUserRoles as t1 " +
                        "INNER JOIN TelegramRoles as t2 ON t1.TelegramRoleId = t2.Id) " +
                        "INNER JOIN TelegramBotUsers as t3 ON t1.TelegramBotUserId = t3.Id) " +
                        "INNER JOIN TelegramUsers as t4 ON t3.TelegramUserId = t4.Id) " +
                        $"WHERE t2.Name = '{role}' AND t3.TelegramBotId = {_botId} AND t4.ChatId = {chatId}";

                await command.ExecuteNonQueryAsync();
            }

            if (connection.State == ConnectionState.Open) await connection.CloseAsync();
        }

        public async Task<List<TRole>> GetUserRoles(long chatId)
        {
            return await _db.Set<TRole>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM (((TelegramRoles as t1 "
              + "INNER JOIN TelegramBotUserRoles as t2 ON t1.Id = t2.TelegramRoleId) "
              + "INNER JOIN TelegramBotUsers as t3 ON t3.Id = t2.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers as t4 ON t4.Id = t3.TelegramUserId) "
              + $"WHERE t4.ChatId = {chatId} AND t3.TelegramBotId = {_botId}").ToListAsync();
        }

        public async Task<List<TRole>> GetUserRoles(string username)
        {
            return await _db.Set<TRole>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM (((TelegramRoles as t1 "
              + "INNER JOIN TelegramBotUserRoles as t2 ON t1.Id = t2.TelegramRoleId) "
              + "INNER JOIN TelegramBotUsers as t3 ON t3.Id = t2.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers as t4 ON t4.Id = t3.TelegramUserId) "
              + $"WHERE t4.Username = '{username}' AND t3.TelegramBotId = {_botId}").ToListAsync();
        }
    }
}
