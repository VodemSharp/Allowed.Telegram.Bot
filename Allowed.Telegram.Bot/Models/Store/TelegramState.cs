using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Models.Store
{
    public partial class TelegramState
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TelegramUser")]
        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; }

        [ForeignKey("TelegramBot")]
        public int? TelegramBotId { get; set; }
        public TelegramBot TelegramBot { get; set; }

        public string Value { get; set; }
    }
}
