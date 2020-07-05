using System;
using System.ComponentModel.DataAnnotations;

namespace Allowed.Telegram.Bot.Models.Store
{
    public class TelegramRole<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }
        public string Name { get; set; }
    }
}
