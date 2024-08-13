using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Managers;

public class TelegramWebHookManager : ITelegramManager
{
    private const string DefaultRoute = "telegram";
    protected readonly ClientsCollection ClientsCollection;
    protected readonly ILogger<TelegramWebHookManager> Logger;
    protected readonly string Route;
    protected readonly TelegramWebHookOptions TelegramWebHookOptions;

    public TelegramWebHookManager(ILoggerFactory loggerFactory, IOptions<TelegramWebHookOptions> telegramWebHookOptions,
        ClientsCollection clientsCollection)
    {
        Logger = loggerFactory.CreateLogger<TelegramWebHookManager>();

        TelegramWebHookOptions = telegramWebHookOptions.Value;
        Route = TelegramWebHookOptions.Route ?? DefaultRoute;
        ClientsCollection = clientsCollection;
    }

    public virtual async Task Start(ClientItem client)
    {
        if (ClientsCollection.Clients.Any(c => c.Options.Name == client.Options.Name))
        {
            Logger.LogWarning("Telegram bot has already been started!");
            return;
        }

        ClientsCollection.Clients.Add(client);

        var webhookAddress = @$"{client.Options.Host}/{Route}/{client.Options.Token}";

        if (TelegramWebHookOptions.DeleteOldHooks)
            await client.Client.DeleteWebhookAsync();
        await client.Client.SetWebhookAsync(
            webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>());
    }

    public virtual async Task Stop(IEnumerable<string> names)
    {
        foreach (var name in names) await Stop(name);
    }

    public Task Stop(string name)
    {
        var client = ClientsCollection.Clients.SingleOrDefault(c => c.Options.Name == name);
        if (client == null)
        {
            Logger.LogWarning("Telegram bot has already been stopped!");
            return Task.CompletedTask;
        }

        client.Client.DeleteWebhookAsync();
        ClientsCollection.Clients.Remove(client);
        return Task.CompletedTask;
    }

    public virtual async Task Start(IEnumerable<ClientItem> clients)
    {
        foreach (var client in clients) await Start(client);
    }
}