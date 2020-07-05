using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Models.Store
{
    public partial class TelegramState<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }

        [ForeignKey("TelegramUser")]
        public TKey TelegramUserId { get; set; }
        public TelegramUser<TKey> TelegramUser { get; set; }

        [ForeignKey("TelegramBot")]
        public TKey TelegramBotId { get; set; }
        public TelegramBot<TKey> TelegramBot { get; set; }

        public string Value { get; set; }
    }
}
