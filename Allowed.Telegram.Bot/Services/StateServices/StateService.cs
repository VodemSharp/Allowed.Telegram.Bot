using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Services.StateServices
{
    public class StateService<TKey, TState> : IStateService<TKey, TState>
        where TKey : IEquatable<TKey>
        where TState : TelegramState<TKey>
    {
        private readonly DbContext _db;
        private readonly TKey _botId;

        public StateService(IServiceProvider provider, ContextOptions options, TKey botId)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
            _botId = botId;
        }

        public async Task<TState> GetState(string username)
        {
            return await _db.Set<TState>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM ((TelegramStates AS t1 "
              + "INNER JOIN TelegramBotUsers AS t2 ON t2.Id = t1.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers AS t3 ON t3.Id = t2.TelegramUserId) "
              + $"WHERE t3.Username = '{username}' AND t2.TelegramBotId = {_botId} "
              + "LIMIT 1").OrderBy(s => s.Id).FirstOrDefaultAsync();
        }

        public async Task<TState> GetState(long telegramId)
        {
            return await _db.Set<TState>().FromSqlRaw(
                "SELECT t1.* "
              + "FROM ((TelegramStates AS t1 "
              + "INNER JOIN TelegramBotUsers AS t2 ON t2.Id = t1.TelegramBotUserId) "
              + "INNER JOIN TelegramUsers AS t3 ON t3.Id = t2.TelegramUserId) "
              + $"WHERE t3.TelegramId = {telegramId} AND t2.TelegramBotId = {_botId} "
              + "LIMIT 1").OrderBy(s => s.Id).FirstOrDefaultAsync();
        }

        public async Task SetState(string username, string value)
        {
            TState state = await GetState(username);

            if (state == null)
            {
                TKey botUserId = await _db.GetBotUserId(_botId, username);
                await _db.Set<TState>().AddAsync(
                    ContextBuilder.CreateTelegramState<TKey, TState>(botUserId, value));
            }
            else
            {
                state.Value = value;
                _db.Set<TState>().Update(state);
            }

            await _db.SaveChangesAsync();
        }

        public async Task SetState(long telegramId, string value)
        {
            TState state = await GetState(telegramId);

            if (state == null)
            {
                TKey botUserId = await _db.GetBotUserId(_botId, telegramId);
                await _db.Set<TState>().AddAsync(
                    ContextBuilder.CreateTelegramState<TKey, TState>(botUserId, value));
            }
            else
            {
                state.Value = value;
                _db.Set<TState>().Update(state);
            }

            await _db.SaveChangesAsync();
        }
    }
}
