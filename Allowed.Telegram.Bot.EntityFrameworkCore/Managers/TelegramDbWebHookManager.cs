using System.Net;
using System.Text.Json;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions.Items;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Managers;
using Allowed.Telegram.Bot.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Managers;

public class TelegramDbWebHookManager<TKey, TUser, TRole, TBot> : TelegramWebHookManager
    where TKey : IEquatable<TKey>
    where TUser : TelegramUser<TKey>
    where TRole : TelegramRole<TKey>
    where TBot : TelegramBot<TKey>
{
    private readonly IServiceFactory _serviceFactory;

    public TelegramDbWebHookManager(IServiceProvider services,
        ILoggerFactory loggerFactory,
        IOptions<TelegramWebHookOptions> telegramWebHookOptions)
        : base(services, loggerFactory, telegramWebHookOptions)
    {
        _serviceFactory = (IServiceFactory)services.GetService(typeof(IServiceFactory));
    }

    private async Task InitializeBots(IEnumerable<string> botNames)
    {
        var botsCollection = Services.GetRequiredService<BotsCollection<TKey>>();
        var options = Services.GetRequiredService<ContextOptions>();

        await using var db = (DbContext)Services.GetRequiredService(options.ContextType);
        foreach (var botName in botNames)
        {
            if (!await db.Set<TBot>().AnyAsync(b => b.Name == botName))
            {
                await db.Set<TBot>().AddAsync(ContextBuilder.CreateTelegramBot<TKey, TBot>(botName));
                await db.SaveChangesAsync();
            }

            botsCollection.Values.Add(botName,
                (await db.Set<TBot>().OrderBy(b => b.Id).SingleAsync(b => b.Name == botName)).Id);
        }
    }

    protected override async Task DoWork(CancellationToken stoppingToken)
    {
        try
        {
            var clientsCollection = Services.GetRequiredService<ClientsCollection>();
            await InitializeBots(clientsCollection.Clients.Select(c => c.Options.Name));

            foreach (var client in clientsCollection.Clients)
            {
                var webhookAddress = @$"{client.Options.Host}/{Route}/{client.Options.Token}";

                if (TelegramWebHookOptions.DeleteOldHooks)
                    await client.Client.DeleteWebhookAsync(cancellationToken: stoppingToken);
                await client.Client.SetWebhookAsync(
                    webhookAddress,
                    allowedUpdates: Array.Empty<UpdateType>(),
                    cancellationToken: stoppingToken);

                client.Client.OnApiResponseReceived += async (tgClient, args, cancellationToken) =>
                {
                    try
                    {
                        if (args.ResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                        {
                            var response = await args.ResponseMessage.Content.ReadAsStringAsync(cancellationToken);

                            if (response ==
                                "{\"ok\":false,\"error_code\":403,\"description\":\"Forbidden: bot was blocked by the user\"}")
                            {
                                var request =
                                    await args.ApiRequestEventArgs.HttpRequestMessage?.Content?.ReadAsStringAsync(
                                        cancellationToken)!;

                                var telegramId = JsonSerializer.Deserialize<JsonElement>(request)
                                    .GetProperty("chat_id").GetInt64();

                                await using var scope = Services.CreateAsyncScope();
                                var botsCollections = scope.ServiceProvider.GetRequiredService<BotsCollection<TKey>>();

                                if (tgClient.BotId != null)
                                {
                                    var botId = botsCollections.Values[client.Options.Name];
                                    var userService = _serviceFactory.CreateUserService<TKey, TUser>(botId);
                                    await userService.BlockBot(telegramId);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }
                };
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }
    }
}