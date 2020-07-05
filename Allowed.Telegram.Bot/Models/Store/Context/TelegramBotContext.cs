using Microsoft.EntityFrameworkCore;
using System;

namespace Allowed.Telegram.Bot.Models.Store.Context
{
    public class TelegramBotDbContext<TKey, TUser, TRole, TUserRole, TState, TBot> : DbContext

        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
        where TRole : TelegramRole<TKey>
        where TUserRole : TelegramUserRole<TKey>
        where TState : TelegramState<TKey>
        where TBot : TelegramBot<TKey>
    {
        public TelegramBotDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<TUser> TelegramUsers { get; set; }
        public virtual DbSet<TRole> TelegramRoles { get; set; }
        public virtual DbSet<TUserRole> TelegramUserRoles { get; set; }
        public virtual DbSet<TState> TelegramStates { get; set; }
        public virtual DbSet<TBot> TelegramBots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUserRole>().HasNoKey();
            modelBuilder.Entity<TUser>().HasIndex(u => new { u.ChatId });
        }
    }

    public class TelegramBotDbContext<TKey> : TelegramBotDbContext<TKey, TelegramUser<TKey>,
        TelegramRole<TKey>, TelegramUserRole<TKey>, TelegramState<TKey>, TelegramBot<TKey>>

        where TKey : IEquatable<TKey>
    {
        public TelegramBotDbContext(DbContextOptions options) : base(options) { }
    }

    public class TelegramBotDbContext : TelegramBotDbContext<int, TelegramUser<int>,
        TelegramRole<int>, TelegramUserRole<int>, TelegramState<int>, TelegramBot<int>>
    {
        public TelegramBotDbContext(DbContextOptions options) : base(options) { }
    }
}
