using Allowed.Telegram.Bot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Contexts;

public abstract class TelegramBotDbContext<TKey, TUser, TRole, TBot, TBotUser, TBotUserRole> : DbContext
    where TKey : IEquatable<TKey>
    where TUser : TelegramUser<TKey>
    where TRole : TelegramRole<TKey>
    where TBot : TelegramBot<TKey>
    where TBotUser : TelegramBotUser<TKey>
    where TBotUserRole : TelegramBotUserRole<TKey>
{
    protected TelegramBotDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<TUser> TelegramUsers { get; set; }
    public virtual DbSet<TRole> TelegramRoles { get; set; }
    public virtual DbSet<TBot> TelegramBots { get; set; }
    public virtual DbSet<TBotUser> TelegramBotUsers { get; set; }
    public virtual DbSet<TBotUserRole> TelegramBotUserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<TelegramUser<TKey>>();
        builder.Ignore<TelegramRole<TKey>>();
        builder.Ignore<TelegramBot<TKey>>();
        builder.Ignore<TelegramBotUser<TKey>>();
        builder.Ignore<TelegramBotUserRole<TKey>>();

        // TelegramUser
        builder.Entity<TUser>().ToTable("TelegramUsers");
        builder.Entity<TUser>().HasKey(u => u.Id);

        builder.Entity<TUser>().HasIndex(u => u.TelegramId);
        builder.Entity<TUser>().HasIndex(u => u.Username);

        // TelegramRole
        builder.Entity<TRole>().ToTable("TelegramRoles");
        builder.Entity<TRole>().HasKey(r => r.Id);

        // TelegramBot
        builder.Entity<TBot>().ToTable("TelegramBots");
        builder.Entity<TBot>().HasKey(b => b.Id);

        // TelegramBotUser
        builder.Entity<TBotUser>().ToTable("TelegramBotUsers");
        builder.Entity<TBotUser>().HasKey(bu => bu.Id);

        builder.Entity<TBotUser>().HasOne<TUser>().WithMany()
            .HasForeignKey(bu => bu.TelegramUserId);

        builder.Entity<TBotUser>().HasOne<TBot>().WithMany()
            .HasForeignKey(bu => bu.TelegramBotId);

        // TelegramBotUserRole
        builder.Entity<TBotUserRole>().ToTable("TelegramBotUserRoles");
        builder.Entity<TBotUserRole>().HasKey(bur => new { bur.TelegramBotUserId, bur.TelegramRoleId });

        builder.Entity<TBotUserRole>().HasOne<TBotUser>().WithMany()
            .HasForeignKey(bur => bur.TelegramBotUserId);

        builder.Entity<TBotUserRole>().HasOne<TRole>().WithMany()
            .HasForeignKey(bur => bur.TelegramRoleId);
    }
}