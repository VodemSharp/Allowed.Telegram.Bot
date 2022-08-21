using Allowed.Telegram.Bot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Helpers;

public static class LinqHelper
{
    public static async Task<TKey> GetBotUserId<TKey>(IEnumerable<TelegramUser<TKey>> users,
        IQueryable<TelegramBotUser<TKey>> botUsers, TKey botId, long telegramId)
        where TKey : IEquatable<TKey>
    {
        return await (from bu in botUsers
                join u in users on bu.TelegramUserId equals u.Id
                where bu.TelegramBotId.Equals(botId) && u.TelegramId == telegramId
                select bu.Id)
            .SingleAsync();
    }

    public static async Task<TKey> GetBotUserId<TKey>(IEnumerable<TelegramUser<TKey>> users,
        IQueryable<TelegramBotUser<TKey>> botUsers, TKey botId, string username)
        where TKey : IEquatable<TKey>
    {
        return await (from bu in botUsers
                join u in users on bu.TelegramUserId equals u.Id
                where bu.TelegramBotId.Equals(botId) && u.Username == username
                select bu.Id)
            .SingleAsync();
    }
}