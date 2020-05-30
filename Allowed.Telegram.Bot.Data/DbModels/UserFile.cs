using Allowed.Telegram.Bot.Models.Store;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allowed.Telegram.Bot.Data.DbModels
{
    public class UserFile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TelegramUser")]
        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; }

        public string Type { get; set; }
        public string Value { get; set; }
    }
}
