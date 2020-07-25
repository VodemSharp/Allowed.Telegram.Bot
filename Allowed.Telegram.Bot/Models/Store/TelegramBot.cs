using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Models.Store
{
    [Table("TelegramBots")]
    public partial class TelegramBot<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }
        public string Name { get; set; }
    }
}
