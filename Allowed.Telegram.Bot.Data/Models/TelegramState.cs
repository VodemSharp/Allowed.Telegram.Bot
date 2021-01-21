using System;

namespace Allowed.Telegram.Bot.Data.Models
{
    public class TelegramState<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }

        public virtual TKey TelegramBotUserId { get; set; }

        public virtual string Value { get; set; }
    }
}
