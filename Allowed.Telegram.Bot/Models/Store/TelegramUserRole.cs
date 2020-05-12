using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Models.Store
{
    public class TelegramUserRole
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TelegramUser")]
        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; }

        [ForeignKey("TelegramRole")]
        public int TelegramRoleId { get; set; }
        public TelegramRole TelegramRole { get; set; }
    }
}
