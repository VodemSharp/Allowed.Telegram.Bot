using Allowed.Telegram.Bot.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;

public static class DbContextExtensions
{
    public static void AddTelegramTables<TKey, TUser, TRole, TBot, TBotUser, TBotUserRole>(this ModelBuilder builder)
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser
        where TBot : TelegramBot
        where TBotUser : TelegramBotUser
        where TRole : TelegramRole<TKey>
        where TBotUserRole : TelegramBotUserRole<TKey>
    {
        builder.Ignore<TelegramUser>();
        builder.Ignore<TelegramBot>();
        builder.Ignore<TelegramBotUser>();
        builder.Ignore<TelegramRole<TKey>>();
        builder.Ignore<TelegramBotUserRole<TKey>>();

        // TelegramUser
        var tUserEntity = builder.Entity<TUser>();
        tUserEntity.HasKey(u => u.Id);
        tUserEntity.HasIndex(u => u.Username);

        // TelegramRole
        var tRoleEntity = builder.Entity<TRole>();
        tRoleEntity.HasKey(r => r.Id);
        tRoleEntity.HasAlternateKey(r => r.Name);

        // TelegramBot
        var tBotEntity = builder.Entity<TBot>();
        tBotEntity.HasKey(b => b.Id);

        // TelegramBotUser
        var tBotUserEntity = builder.Entity<TBotUser>();

        tBotUserEntity.HasKey(bu => new { bu.TelegramBotId, bu.TelegramUserId });

        var botUserForeignKeys = tBotUserEntity.Metadata.GetForeignKeys().ToList();

        if (!botUserForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramUserId")))
            tBotUserEntity.HasOne<TUser>().WithMany().HasForeignKey(bu => bu.TelegramUserId);

        if (!botUserForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramBotId")))
            tBotUserEntity.HasOne<TBot>().WithMany().HasForeignKey(bu => bu.TelegramBotId);

        // TelegramBotUserRole
        var tBotUserRoleEntity = builder.Entity<TBotUserRole>();

        tBotUserRoleEntity.HasKey(bur => new { bur.TelegramBotId, bur.TelegramUserId, bur.TelegramRoleId });

        var botUserRoleForeignKeys = tBotUserRoleEntity.Metadata.GetForeignKeys().ToList();

        if (!botUserRoleForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramBotUserId")))
        {
            tBotUserRoleEntity.HasOne<TBotUser>().WithMany()
                .HasForeignKey(bur => new { bur.TelegramBotId, bur.TelegramUserId });
        }

        if (!botUserRoleForeignKeys.Any(x => x.Properties.Any(p => p.IsForeignKey() && p.Name == "TelegramRoleId")))
            tBotUserRoleEntity.HasOne<TRole>().WithMany().HasForeignKey(bur => bur.TelegramRoleId);
    }
}