using System;

namespace Allowed.Telegram.Bot.Data.Models;

public class TelegramBotUserRole<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual TKey TelegramBotUserId { get; set; }
    public virtual TKey TelegramRoleId { get; set; }
}