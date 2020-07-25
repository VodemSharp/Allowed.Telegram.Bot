using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Models.Store
{
    [Table("TelegramRoles")]
    public class TelegramRole<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }
        public string Name { get; set; }
    }
}
