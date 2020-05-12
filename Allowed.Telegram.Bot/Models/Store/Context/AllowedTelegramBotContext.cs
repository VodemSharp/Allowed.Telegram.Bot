using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Models.Store.Context
{
    public class AllowedTelegramBotDbContext : DbContext
    {
        public AllowedTelegramBotDbContext(DbContextOptions options) : base(options) { }

        public DbSet<TelegramState> TelegramStates { get; set; }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<TelegramBot> TelegramBots { get; set; }
        public DbSet<TelegramRole> TelegramRoles { get; set; }
        public DbSet<TelegramUserRole> TelegramUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TelegramUser>().HasIndex(u => new { u.ChatId });
        }
    }
}
