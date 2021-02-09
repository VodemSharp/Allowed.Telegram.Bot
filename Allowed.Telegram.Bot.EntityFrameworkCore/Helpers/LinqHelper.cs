using Allowed.Telegram.Bot.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Helpers
{
    public class LinqHelper
    {
        public async Task<TKey> GetBotUserId<TKey>(IQueryable<TelegramUser<TKey>> users,
            IQueryable<TelegramBotUser<TKey>> botUsers, TKey botId, long telegramId)

            where TKey : IEquatable<TKey>
        {
            return await (from u in users
                          join bu in botUsers on u.Id equals bu.TelegramUserId
                          where u.TelegramId == telegramId && bu.TelegramBotId.Equals(botId)
                          select u.Id)
                  .FirstOrDefaultAsync();
        }

        public async Task<TKey> GetBotUserId<TKey>(IQueryable<TelegramUser<TKey>> users,
                IQueryable<TelegramBotUser<TKey>> botUsers, TKey botId, string username)

            where TKey : IEquatable<TKey>
        {
            return await (from u in users
                          join bu in botUsers on u.Id equals bu.TelegramUserId
                          where u.Username == username && bu.TelegramBotId.Equals(botId)
                          select u.Id)
                  .FirstOrDefaultAsync();
        }
    }
}
