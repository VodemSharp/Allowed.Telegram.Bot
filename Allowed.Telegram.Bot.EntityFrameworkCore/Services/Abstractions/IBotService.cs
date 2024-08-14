namespace Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;

public interface IBotService
{
    Task Add(long telegramId);
    Task<bool> Any(long telegramId);
}