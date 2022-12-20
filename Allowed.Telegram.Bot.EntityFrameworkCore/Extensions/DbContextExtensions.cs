using Allowed.Telegram.Bot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;

public static class DbContextExtensions
{
    public static IQueryable Set(this DbContext context, Type t)
    {
        var set = context.GetType().GetMethods().Single(m => m.Name == "Set" && m.GetParameters().Length == 0);
        return (IQueryable)set.MakeGenericMethod(t).Invoke(context, null);
    }

    public static void AddTelegramTables<TKey, TUser, TRole, TBot, TBotUser, TBotUserRole>(this ModelBuilder builder)
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
        where TRole : TelegramRole<TKey>
        where TBot : TelegramBot<TKey>
        where TBotUser : TelegramBotUser<TKey>
        where TBotUserRole : TelegramBotUserRole<TKey>
    {
        builder.Ignore<TelegramUser<TKey>>();
        builder.Ignore<TelegramRole<TKey>>();
        builder.Ignore<TelegramBot<TKey>>();
        builder.Ignore<TelegramBotUser<TKey>>();
        builder.Ignore<TelegramBotUserRole<TKey>>();

        // TelegramUser
        builder.Entity<TUser>().HasKey(u => u.Id);

        builder.Entity<TUser>().HasIndex(u => u.TelegramId).IsUnique();
        builder.Entity<TUser>().HasIndex(u => u.Username);

        // TelegramRole
        builder.Entity<TRole>().HasKey(r => r.Id);

        // TelegramBot
        // builder.Entity<TBot>().ToTable("TelegramBots");
        builder.Entity<TBot>().HasKey(b => b.Id);

        // TelegramBotUser
        builder.Entity<TBotUser>().HasKey(bu => bu.Id);

        builder.Entity<TBotUser>().HasOne<TUser>().WithMany()
            .HasForeignKey(bu => bu.TelegramUserId);

        builder.Entity<TBotUser>().HasOne<TBot>().WithMany()
            .HasForeignKey(bu => bu.TelegramBotId);

        // TelegramBotUserRole
        builder.Entity<TBotUserRole>().HasKey(bur => new { bur.TelegramBotUserId, bur.TelegramRoleId });

        builder.Entity<TBotUserRole>().HasOne<TBotUser>().WithMany()
            .HasForeignKey(bur => bur.TelegramBotUserId);

        builder.Entity<TBotUserRole>().HasOne<TRole>().WithMany()
            .HasForeignKey(bur => bur.TelegramRoleId);
    }
}