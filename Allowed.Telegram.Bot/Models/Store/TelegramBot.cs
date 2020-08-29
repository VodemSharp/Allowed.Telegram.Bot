using System;

namespace Allowed.Telegram.Bot.Models.Store
{
    public class TelegramBot<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }
        public virtual string Name { get; set; }
    }
}
