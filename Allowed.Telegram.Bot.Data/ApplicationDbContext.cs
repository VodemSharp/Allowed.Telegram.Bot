using Allowed.Telegram.Bot.Data.DbModels;
using Allowed.Telegram.Bot.Models.Store.Context;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Sample.Data
{
    public class ApplicationDbContext : TelegramBotDbContext
    {
        public DbSet<UserFile> UserFiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
