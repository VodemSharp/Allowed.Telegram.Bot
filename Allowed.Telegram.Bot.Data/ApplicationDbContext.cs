using Allowed.Telegram.Bot.Models.Store;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Sample.Data
{
    public class ApplicationDbContext : AllowedTelegramBotDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
