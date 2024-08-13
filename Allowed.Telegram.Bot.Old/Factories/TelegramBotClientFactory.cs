using Allowed.Telegram.Bot.Clients;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Options;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Factories;

public class TelegramBotClientFactory
{
    public static ClientItem CreateClient(SimpleTelegramBotClientOptions options, CancellationTokenSource source = null)
    {
        source ??= new CancellationTokenSource();
        return new ClientItem
        {
            Client = options.GetType() == typeof(SafeTelegramBotClientOptions)
                ? new SafeTelegramBotClient((SafeTelegramBotClientOptions)options)
                : new TelegramBotClient(options),
            Options = options,
            CancellationTokenSource = source
        };
    }
}