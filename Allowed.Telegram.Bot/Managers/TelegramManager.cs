using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Managers;

public class TelegramManager : ITelegramManager
{
    protected readonly ClientsCollection ClientsCollection;
    protected readonly ControllersCollection ControllersCollection;
    protected readonly ILogger<TelegramManager> Logger;

    public TelegramManager(IServiceProvider services,
        ControllersCollection controllersCollection,
        ClientsCollection clientsCollection,
        ILoggerFactory loggerFactory)
    {
        ControllersCollection = controllersCollection;
        ClientsCollection = clientsCollection;
        Logger = loggerFactory.CreateLogger<TelegramManager>();
        Services = services;
    }

    protected IServiceProvider Services { get; }

    public virtual async Task Start(ClientItem client)
    {
        if (ClientsCollection.Clients.Any(c => c.Options.Name == client.Options.Name))
        {
            Logger.LogWarning("Telegram bot has already been started!");
            return;
        }

        ClientsCollection.Clients.Add(client);

        async void UpdateHandler(ITelegramBotClient tgClient, Update update, CancellationToken token)
        {
            await using var scope = Services.CreateAsyncScope();
            await new MessageHandler(ControllersCollection, tgClient, client.Options, scope.ServiceProvider)
                .OnUpdate(tgClient, update, token);
        }

        await client.Client.DeleteWebhookAsync(cancellationToken: client.CancellationTokenSource.Token);
        client.Client.StartReceiving(UpdateHandler,
            (tgClient, exception, _) => Logger.LogError("{botId}:\n{exception}",
                tgClient.BotId, exception.ToString()),
            new ReceiverOptions(), client.CancellationTokenSource.Token);
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

        client.CancellationTokenSource.Cancel();
        ClientsCollection.Clients.Remove(client);
        return Task.CompletedTask;
    }

    public virtual async Task Start(IEnumerable<ClientItem> clients)
    {
        foreach (var client in clients) await Start(client);
    }
}