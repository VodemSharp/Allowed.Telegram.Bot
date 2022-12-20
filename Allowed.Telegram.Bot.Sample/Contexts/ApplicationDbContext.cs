using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.Sample.DbModels;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.Sample.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public virtual DbSet<ApplicationTgUser> TelegramUsers { get; set; }
    public virtual DbSet<ApplicationTgRole> TelegramRoles { get; set; }
    public virtual DbSet<ApplicationTgBot> TelegramBots { get; set; }
    public virtual DbSet<ApplicationTgBotUser> TelegramBotUsers { get; set; }
    public virtual DbSet<ApplicationTgBotUserRole> TelegramBotUserRoles { get; set; }

    public DbSet<UserFile> UserFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.AddTelegramTables<int, ApplicationTgUser, ApplicationTgRole, ApplicationTgBot, ApplicationTgBotUser,
            ApplicationTgBotUserRole>();

        // UserFile
        builder.Entity<UserFile>().HasKey(uf => uf.Id);

        builder.Entity<UserFile>().HasOne(uf => uf.TelegramUser)
            .WithMany(uf => uf.UserFiles)
            .HasForeignKey(uf => uf.TelegramUserId);
    }
}