using Allowed.Telegram.Bot.Handlers;

namespace Allowed.Telegram.Bot.Managers;

public interface ITelegramManager
{
    Task Start(TelegramHandler telegramHandlers);
    Task Start(IEnumerable<TelegramHandler> clients);
    Task Stop(long telegramId);
    Task Stop(IEnumerable<long> telegramIds);
}