using Allowed.Telegram.Bot.Data.Entities;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services;

public class UserService<TContext, TUser, TBotUser>(TContext db) : IUserService
    where TContext : DbContext
    where TUser : TelegramUser, new()
    where TBotUser : TelegramBotUser, new()
{
    private readonly DbSet<TBotUser> _botUsers = db.Set<TBotUser>();
    private readonly DbSet<TUser> _users = db.Set<TUser>();

    #region Users

    public async Task<bool> Any(long id)
    {
        return await _users.AnyAsync(x => x.Id == id);
    }

    private static void MapUser(TUser tUser, User user)
    {
        tUser.IsBot = user.IsBot;
        tUser.FirstName = user.FirstName;
        tUser.LastName = user.LastName;
        tUser.Username = user.Username;
        tUser.LanguageCode = user.LanguageCode;
        tUser.IsPremium = user.IsPremium;
    }

    public async Task Add(User user)
    {
        var newUser = new TUser
        {
            Id = user.Id,
            UpdatedAt = DateTime.UtcNow
        };

        MapUser(newUser, user);

        await _users.AddAsync(newUser);
        await db.SaveChangesAsync();
    }

    public async Task Update(User user)
    {
        var tUser = await _users.SingleAsync(x => x.Id == user.Id);

        MapUser(tUser, user);
        tUser.UpdatedAt = DateTime.UtcNow;

        _users.Update(tUser);
        await db.SaveChangesAsync();
    }

    #endregion

    #region BotUsers

    public async Task<bool> Any(long botId, long userId)
    {
        return await _botUsers.AnyAsync(x => x.TelegramBotId == botId && x.TelegramUserId == userId);
    }

    public async Task<string?> GetState(long botId, long userId)
    {
        return await _botUsers.Where(x => x.TelegramBotId == botId && x.TelegramUserId == userId)
            .Select(x => x.State).SingleAsync();
    }

    public async Task SetState(long botId, long userId, string? state)
    {
        var botUser = await _botUsers.SingleAsync(x => x.TelegramBotId == botId && x.TelegramUserId == userId);
        botUser.State = state;
        _botUsers.Update(botUser);
        await db.SaveChangesAsync();
    }

    public async Task SetBlocked(long botId, long userId, bool blocked = true)
    {
        var botUser = await _botUsers.Where(x => x.TelegramBotId == botId && x.TelegramUserId == userId).SingleAsync();
        botUser.Blocked = blocked;
        _botUsers.Update(botUser);
        await db.SaveChangesAsync();
    }

    public async Task Add(long botId, long userId, User user)
    {
        await _botUsers.AddAsync(new TBotUser
        {
            TelegramUserId = userId,
            TelegramBotId = botId,
            Blocked = false,
            State = null,
            AddedToAttachmentMenu = user.AddedToAttachmentMenu,
            UpdatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task Update(long botId, long userId, User user)
    {
        var botUser = await _botUsers.SingleAsync(x => x.TelegramBotId == botId && x.TelegramUserId == userId);

        botUser.Blocked = false;
        botUser.AddedToAttachmentMenu = user.AddedToAttachmentMenu;
        botUser.UpdatedAt = DateTime.UtcNow;

        _botUsers.Update(botUser);
        await db.SaveChangesAsync();
    }

    #endregion
}