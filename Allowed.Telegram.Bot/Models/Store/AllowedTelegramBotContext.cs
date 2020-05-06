using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Models.Store
{
    public class AllowedTelegramBotDbContext : DbContext
    {
        public AllowedTelegramBotDbContext(DbContextOptions options) : base(options) { }

        public DbSet<TelegramState> TelegramStates { get; set; }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<TelegramBot> TelegramBots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TelegramUser>().HasIndex(u => new { u.ChatId });
        }
    }
}
