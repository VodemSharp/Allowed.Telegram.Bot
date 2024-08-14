using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;

public interface IUserService
{
    Task<bool> Any(long id);
    Task Add(User user);
    Task Update(User user);

    Task<bool> Any(long botId, long userId);
    Task<string?> GetState(long botId, long userId);
    Task SetState(long botId, long userId, string? state);
    Task SetBlocked(long botId, long userId, bool blocked = true);
    Task Add(long botId, long userId, User user);
    Task Update(long botId, long userId, User user);
}