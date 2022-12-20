using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Managers;

public class TelegramWebHookManager : BackgroundService
{
    private const string DefaultRoute = "telegram";
    protected readonly ILogger<TelegramWebHookManager> Logger;
    protected readonly string Route;
    protected readonly IServiceProvider Services;
    protected readonly TelegramWebHookOptions TelegramWebHookOptions;

    public TelegramWebHookManager(IServiceProvider services,
        ILoggerFactory loggerFactory, IOptions<TelegramWebHookOptions> telegramWebHookOptions)
    {
        Services = services;
        Logger = loggerFactory.CreateLogger<TelegramWebHookManager>();

        TelegramWebHookOptions = telegramWebHookOptions.Value;
        Route = TelegramWebHookOptions.Route ?? DefaultRoute;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = Services.CreateScope();
        var clientsCollection = scope.ServiceProvider.GetRequiredService<ClientsCollection>();

        foreach (var client in clientsCollection!.Clients)
            await client.Client.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }

    protected virtual async Task DoWork(CancellationToken stoppingToken)
    {
        using var scope = Services.CreateScope();
        var clientsCollection = scope.ServiceProvider.GetRequiredService<ClientsCollection>();

        foreach (var client in clientsCollection!.Clients)
        {
            var webhookAddress = @$"{client.Options.Host}/{Route}/{client.Options.Token}";

            if (TelegramWebHookOptions.DeleteOldHooks)
                await client.Client.DeleteWebhookAsync(cancellationToken: stoppingToken);
            await client.Client.SetWebhookAsync(
                webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: stoppingToken);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}