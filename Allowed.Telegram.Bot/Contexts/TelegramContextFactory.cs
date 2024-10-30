using Allowed.Telegram.Bot.Clients;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Contexts;

public static class TelegramContextFactory
{
    public static TelegramContext CreateHandler(TelegramBotClientOptions options,
        CancellationTokenSource? source = null)
    {
        source ??= new CancellationTokenSource();
        return new TelegramContext
        {
            Client = options.GetType() == typeof(SafeTelegramBotClientOptions)
                ? new SafeTelegramBotClient((SafeTelegramBotClientOptions)options)
                : new TelegramBotClient(options),
            Options = options,
            CancellationTokenSource = source
        };
    }
}