namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;

public interface IRoleService
{
    Task<bool> Any(string role);
    Task Add(string role);
    Task Remove(string role);

    Task<bool> Any(long botId, long userId, string roleName);
    Task<List<string>> Get(long botId, long telegramId);
    Task Add(long botId, long userId, string roleName);
    Task Remove(long botId, long userId, string roleName);
}