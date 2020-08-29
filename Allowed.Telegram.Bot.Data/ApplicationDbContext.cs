using Allowed.Telegram.Bot.Data.DbModels;
using Allowed.Telegram.Bot.Data.DbModels.Allowed;
using Allowed.Telegram.Bot.Models.Store.Context;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Sample.Data
{
    public class ApplicationDbContext :
        TelegramBotDbContext<int, ApplicationTgUser, ApplicationTgRole, ApplicationTgBot, ApplicationTgBotUser, ApplicationTgBotUserRole, ApplicationTgState>
    {
        public DbSet<UserFile> UserFiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // UserFile
            builder.Entity<UserFile>().HasKey(uf => uf.Id);

            builder.Entity<UserFile>().HasOne(uf => uf.TelegramUser)
                                      .WithMany(uf => uf.UserFiles)
                                      .HasForeignKey(uf => uf.TelegramUserId);
        }
    }
}
