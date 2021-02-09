using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.EntityFrameworkCore.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services
{
    public class StateService<TKey, TState> : IStateService<TKey, TState>
        where TKey : IEquatable<TKey>
        where TState : TelegramState<TKey>
    {
        private readonly LinqHelper _linqHelper;
        private readonly DbContext _db;
        private readonly TKey _botId;

        private readonly IQueryable<TelegramUser<TKey>> _users;
        private readonly IQueryable<TelegramBotUser<TKey>> _botUsers;
        private readonly IQueryable<TelegramState<TKey>> _states;

        public StateService(IServiceProvider provider, ContextOptions options, TKey botId)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
            _botId = botId;
            _linqHelper = new LinqHelper();

            _users = _db.Set(options.UserType).Cast<TelegramUser<TKey>>();
            _botUsers = _db.Set(options.BotUserType).Cast<TelegramBotUser<TKey>>();
            _states = _db.Set(options.StateType).Cast<TelegramState<TKey>>();
        }

        public async Task<TState> GetState(string username)
        {
            return await (from s in _states
                          join bu in _botUsers on s.TelegramBotUserId equals bu.Id
                          join u in _users on bu.TelegramUserId equals u.Id
                          where u.Username == username && bu.TelegramBotId.Equals(_botId)
                          select s)
                    .Cast<TState>().FirstOrDefaultAsync();
        }

        public async Task<TState> GetState(long telegramId)
        {
            return await (from s in _states
                          join bu in _botUsers on s.TelegramBotUserId equals bu.Id
                          join u in _users on bu.TelegramUserId equals u.Id
                          where u.TelegramId == telegramId && bu.TelegramBotId.Equals(_botId)
                          select s)
                    .Cast<TState>().FirstOrDefaultAsync();
        }

        public async Task SetState(string username, string value)
        {
            TState state = await GetState(username);

            if (state == null)
            {
                TKey botUserId = await _linqHelper.GetBotUserId(_users, _botUsers, _botId, username);
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
                TKey botUserId = await _linqHelper.GetBotUserId(_users, _botUsers, _botId, telegramId);
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
