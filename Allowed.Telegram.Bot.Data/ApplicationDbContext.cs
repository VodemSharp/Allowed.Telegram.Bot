using Allowed.Telegram.Bot.Models.Store.Context;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Sample.Data
{
    public class ApplicationDbContext : AllowedTelegramBotDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
