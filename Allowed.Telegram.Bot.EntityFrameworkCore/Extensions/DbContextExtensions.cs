using System.Linq.Expressions;
using Allowed.Telegram.Bot.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        var tUserEntity = builder.Entity<TUser>();
        tUserEntity.HasKey(u => u.Id);

        tUserEntity.HasIndex(u => u.TelegramId).IsUnique();
        tUserEntity.HasIndex(u => u.Username);

        // TelegramRole
        builder.Entity<TRole>().HasKey(r => r.Id);

        // TelegramBot
        builder.Entity<TBot>().HasKey(b => b.Id);

        // TelegramBotUser
        var tBotUserEntity = builder.Entity<TBotUser>();

        tBotUserEntity.HasKey(bu => bu.Id);

        var botUserForeignKeys = tBotUserEntity.Metadata.GetForeignKeys().ToList();

        if (!botUserForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramUserId")))
            tBotUserEntity.HasOne<TUser>().WithMany().HasForeignKey(bu => bu.TelegramUserId);

        if (!botUserForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramBotId")))
            tBotUserEntity.HasOne<TBot>().WithMany().HasForeignKey(bu => bu.TelegramBotId);

        // TelegramBotUserRole
        var tBotUserRoleEntity = builder.Entity<TBotUserRole>();

        tBotUserRoleEntity.HasKey(bur => new { bur.TelegramBotUserId, bur.TelegramRoleId });

        var botUserRoleForeignKeys = tBotUserRoleEntity.Metadata.GetForeignKeys().ToList();

        if (!botUserRoleForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramBotUserId")))
            tBotUserRoleEntity.HasOne<TBotUser>().WithMany().HasForeignKey(bur => bur.TelegramBotUserId);

        if (!botUserRoleForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramRoleId")))
            tBotUserRoleEntity.HasOne<TRole>().WithMany().HasForeignKey(bur => bur.TelegramRoleId);
    }
}