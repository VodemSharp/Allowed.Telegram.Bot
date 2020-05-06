using System.ComponentModel.DataAnnotations;

namespace Allowed.Telegram.Bot.Models.Store
{
    public partial class TelegramUser
    {
        [Key]
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Username { get; set; }
    }
}
