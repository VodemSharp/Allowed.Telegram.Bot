using System;

namespace Allowed.Telegram.Bot.Models.Store
{
    public class TelegramBotUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }

        public virtual TKey TelegramUserId { get; set; }
        public virtual TKey TelegramBotId { get; set; }
    }
}
