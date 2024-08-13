using Allowed.Telegram.Bot.Clients;
using Allowed.Telegram.Bot.Clients.Options;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Handlers;

public static class TelegramHandlerFactory
{
    public static TelegramHandler CreateClient(SimpleTelegramBotClientOptions options,
        CancellationTokenSource? source = null)
    {
        source ??= new CancellationTokenSource();
        return new TelegramHandler
        {
            Client = options.GetType() == typeof(SafeTelegramBotClientOptions)
                ? new SafeTelegramBotClient((SafeTelegramBotClientOptions)options)
                : new TelegramBotClient(options),
            Options = options,
            CancellationTokenSource = source
        };
    }
}