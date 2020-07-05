using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;

namespace Allowed.Telegram.Bot.Services.StateServices
{
    public class StateService<TState> : IStateService<TState>
        where TState : class
    {
        private readonly DbContext _db;
        private readonly ContextOptions _options;

        public StateService(IServiceProvider provider, ContextOptions options)
        {
            _db = (DbContext)provider.GetService(options.ContextType);
            _options = options;
        }

        public TState GetState(long chatId, string botName = null)
        {
            object user = _db.FromSqlRaw(
                    "SELECT t.* " +
                    "FROM TelegramUsers AS t " +
                   $"WHERE t.ChatId = {chatId} " +
                    "LIMIT 1", _options.UserType)
                .FirstOrDefault();

            if (user == null)
                return null;

            if (string.IsNullOrEmpty(botName))
            {
                return _db.Set<TState>().FromSqlRaw(
                    "SELECT t.* " +
                    "FROM TelegramStates AS t " +
                    "WHERE t.TelegramUserId = {0} " +
                    "LIMIT 1", user.GetProperty("Id")).FirstOrDefault();
            }
            else
            {
                return _db.Set<TState>().FromSqlRaw(
                    "SELECT t.* " +
                    "FROM TelegramStates AS t " +
                    "INNER JOIN TelegramBots AS t0 ON t.TelegramBotId = t0.Id " +
                    "WHERE (t.TelegramUserId = {0}) AND (`t0`.`Name` = '{1}') " +
                    "LIMIT 1", user.GetProperty("Id"), botName).FirstOrDefault();
            }
        }

        private TState GetStateByUserId(object userId, string botName = null)
        {
            if (string.IsNullOrEmpty(botName))
            {
                return _db.Set<TState>().FromSqlRaw(
                "SELECT t.* " +
                "FROM TelegramStates AS t " +
                "WHERE t.TelegramUserId = {0} " +
                "LIMIT 1", userId).FirstOrDefault();
            }
            else
            {
                return _db.Set<TState>().FromSqlRaw(
                    "SELECT t.* " +
                    "FROM TelegramStates AS t " +
                    "INNER JOIN TelegramBots AS t0 ON t.TelegramBotId = t0.Id " +
                    "WHERE (t.TelegramUserId = {0}) AND (`t0`.`Name` = '{1}') " +
                    "LIMIT 1", userId, botName).FirstOrDefault();
            }
        }

        public void SetState(long chatId, string value, string botName = null)
        {
            object telegramUser = _db.Set<TState>().FromSqlRaw(
                "SELECT t.* " +
                "FROM TelegramUsers AS t " +
                "WHERE t.ChatId = {0} " +
                "LIMIT 1", chatId);

            object telegramBot = _db.Set<TState>().FromSqlRaw(
                "SELECT t.* " +
                "FROM TelegramBots AS t " +
                "WHERE t.Name = '{0}' " +
                "LIMIT 1", botName);


            TState state = GetStateByUserId((long)ReflectionHelper.GetProperty(telegramUser, "Id"), botName);

            if (state == null)
            {
                _db.Set<TState>().Add(
                    (TState)ContextBuilder.CreateTelegramState(typeof(TState),
                        ReflectionHelper.GetProperty(telegramUser, "Id"),
                        ReflectionHelper.GetProperty(telegramBot, "Id"),
                        value));
            }
            else
            {
                ReflectionHelper.SetProperty(state, "Value", value);
                _db.Set<TState>().Update(state);
            }

            _db.SaveChanges();
        }
    }
}
