using System;

namespace Allowed.Telegram.Bot.Models.Store
{
    public class TelegramUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }

        public virtual long ChatId { get; set; }
        public virtual string Username { get; set; }
    }
}
