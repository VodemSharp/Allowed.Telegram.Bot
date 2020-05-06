using System.ComponentModel.DataAnnotations;

namespace Allowed.Telegram.Bot.Models.Store
{
    public partial class TelegramBot
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
