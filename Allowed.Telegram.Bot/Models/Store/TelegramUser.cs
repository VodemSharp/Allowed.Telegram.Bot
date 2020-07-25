using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Models.Store
{
    [Table("TelegramUsers")]
    public partial class TelegramUser<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }
        public long ChatId { get; set; }
        public string Username { get; set; }
    }
}
