using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Models.Store
{
    public class TelegramUserRole<TKey>
        where TKey : IEquatable<TKey>
    {
        [ForeignKey("TelegramUser")]
        public TKey TelegramUserId { get; set; }
        public TelegramUser<TKey> TelegramUser { get; set; }

        [ForeignKey("TelegramRole")]
        public TKey TelegramRoleId { get; set; }
        public TelegramRole<TKey> TelegramRole { get; set; }
    }
}
