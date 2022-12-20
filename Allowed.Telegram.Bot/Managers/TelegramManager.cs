using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Managers;

public class TelegramManager : BackgroundService
{
    protected readonly ControllersCollection ControllersCollection;
    protected readonly ILogger<TelegramManager> Logger;

    public TelegramManager(IServiceProvider services,
        ControllersCollection controllersCollection,
        ILoggerFactory loggerFactory)
    {
        Services = services;

        ControllersCollection = controllersCollection;
        Logger = loggerFactory.CreateLogger<TelegramManager>();
    }

    protected IServiceProvider Services { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private MessageHandler GetMessageHandler(ITelegramBotClient client, SimpleTelegramBotClientOptions options)
    {
        return new MessageHandler(ControllersCollection, client, options, Services);
    }

    protected virtual async Task DoWork(CancellationToken stoppingToken)
    {
        var clientsCollection = Services.GetService<ClientsCollection>();

        foreach (var client in clientsCollection!.Clients)
        {
            var messageHandler = GetMessageHandler(client.Client, client.Options);

            var receiverOptions = new ReceiverOptions();

            async void UpdateHandler(ITelegramBotClient tgClient, Update update, CancellationToken token)
            {
                await messageHandler.OnUpdate(tgClient, update, token);
            }

            await client.Client.DeleteWebhookAsync(cancellationToken: stoppingToken);
            client.Client.StartReceiving(UpdateHandler,
                (tgClient, exception, _) => Logger.LogError("{botId}:\n{exception}",
                    tgClient.BotId, exception.ToString()),
                receiverOptions, stoppingToken);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}