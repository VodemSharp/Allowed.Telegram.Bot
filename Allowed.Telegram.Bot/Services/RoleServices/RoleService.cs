using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public async Task<bool> AnyUserRole(long telegramId, string role)
        {
            return await _db.Set<TRole>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM (((TelegramRoles AS t1 "
              + "INNER JOIN TelegramBotUserRoles AS t2 ON t1.Id = t2.TelegramRoleId) "
              + "INNER JOIN TelegramBotUsers AS t3 ON t3.Id = t2.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers AS t4 ON t4.Id = t3.TelegramUserId) "
              + $"WHERE t4.TelegramId = {telegramId} AND t3.TelegramBotId = {_botId} AND t1.Name = '{role}' "
              + "LIMIT 1").AnyAsync();
        }

        public async Task<bool> AnyUserRole(string username, string role)
        {
            return await _db.Set<TRole>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM (((TelegramRoles AS t1 "
              + "INNER JOIN TelegramBotUserRoles AS t2 ON t1.Id = t2.TelegramRoleId) "
              + "INNER JOIN TelegramBotUsers AS t3 ON t3.Id = t2.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers AS t4 ON t4.Id = t3.TelegramUserId) "
              + $"WHERE t4.Username = '{username}' AND t3.TelegramBotId = {_botId} AND t1.Name = '{role}' "
              + "LIMIT 1").AnyAsync();
        }

        public async Task AddUserRole(long telegramId, string role)
        {
            TKey botUserId = await _db.GetBotUserId(_botId, telegramId);
            TKey roleId = (await _db.Set<TRole>().OrderBy(r => r.Id).FirstAsync(r => r.Name == role)).Id;

            await _db.AddAsync(ContextBuilder.CreateTelegramBotUserRole(_options.BotUserRoleType, botUserId, roleId));
            await _db.SaveChangesAsync();
        }

        public async Task AddUserRole(string username, string role)
        {
            TKey botUserId = await _db.GetBotUserId(_botId, username);
            TKey roleId = (await _db.Set<TRole>().OrderBy(r => r.Id).FirstAsync(r => r.Name == role)).Id;

            await _db.AddAsync(ContextBuilder.CreateTelegramBotUserRole(_options.BotUserRoleType, botUserId, roleId));
            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserRole(long telegramId, string role)
        {
            var connection = _db.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                        "DELETE t1 " +
                        "FROM (((TelegramBotUserRoles AS t1 " +
                        "INNER JOIN TelegramRoles AS t2 ON t1.TelegramRoleId = t2.Id) " +
                        "INNER JOIN TelegramBotUsers AS t3 ON t1.TelegramBotUserId = t3.Id) " +
                        "INNER JOIN TelegramUsers AS t4 ON t3.TelegramUserId = t4.Id) " +
                        $"WHERE t2.Name = '{role}' AND t3.TelegramBotId = {_botId} AND t4.TelegramId = {telegramId}";

                await command.ExecuteNonQueryAsync();
            }

            if (connection.State == ConnectionState.Open) await connection.CloseAsync();
        }

        public async Task RemoveUserRole(string username, string role)
        {
            var connection = _db.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                        "DELETE t1 " +
                        "FROM (((TelegramBotUserRoles AS t1 " +
                        "INNER JOIN TelegramRoles AS t2 ON t1.TelegramRoleId = t2.Id) " +
                        "INNER JOIN TelegramBotUsers AS t3 ON t1.TelegramBotUserId = t3.Id) " +
                        "INNER JOIN TelegramUsers AS t4 ON t3.TelegramUserId = t4.Id) " +
                        $"WHERE t2.Name = '{role}' AND t3.TelegramBotId = {_botId} AND t4.Username = '{username}'";

                await command.ExecuteNonQueryAsync();
            }

            if (connection.State == ConnectionState.Open) await connection.CloseAsync();
        }

        public async Task<List<TRole>> GetUserRoles(long telegramId)
        {
            return await _db.Set<TRole>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM (((TelegramRoles AS t1 "
              + "INNER JOIN TelegramBotUserRoles AS t2 ON t1.Id = t2.TelegramRoleId) "
              + "INNER JOIN TelegramBotUsers AS t3 ON t3.Id = t2.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers AS t4 ON t4.Id = t3.TelegramUserId) "
              + $"WHERE t4.TelegramId = {telegramId} AND t3.TelegramBotId = {_botId}").ToListAsync();
        }

        public async Task<List<TRole>> GetUserRoles(string username)
        {
            return await _db.Set<TRole>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM (((TelegramRoles AS t1 "
              + "INNER JOIN TelegramBotUserRoles AS t2 ON t1.Id = t2.TelegramRoleId) "
              + "INNER JOIN TelegramBotUsers AS t3 ON t3.Id = t2.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers AS t4 ON t4.Id = t3.TelegramUserId) "
              + $"WHERE t4.Username = '{username}' AND t3.TelegramBotId = {_botId}").ToListAsync();
        }
    }
}
