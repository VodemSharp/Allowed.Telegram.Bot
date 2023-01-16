using System.Net;
using System.Text.Json;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions.Items;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
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
    private readonly IServiceProvider _services;

    public TelegramDbWebHookManager(IServiceProvider services,
        ILoggerFactory loggerFactory,
        IOptions<TelegramWebHookOptions> telegramWebHookOptions,
        ClientsCollection clientsCollection)
        : base(loggerFactory, telegramWebHookOptions, clientsCollection)
    {
        _services = services;
    }

    private async Task InitializeBots(string botName)
    {
        await using var scope = _services.CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<ContextOptions>();
        await using var db = (DbContext)scope.ServiceProvider.GetRequiredService(options.ContextType);

        if (!await db.Set<TBot>().AnyAsync(b => b.Name == botName))
        {
            await db.Set<TBot>().AddAsync(ContextBuilder.CreateTelegramBot<TKey, TBot>(botName));
            await db.SaveChangesAsync();
        }
    }

    public override async Task Start(ClientItem client)
    {
        if (ClientsCollection.Clients.Any(c => c.Options.Name == client.Options.Name))
        {
            Logger.LogWarning("Telegram bot has already been started!");
            return;
        }

        ClientsCollection.Clients.Add(client);

        await InitializeBots(client.Options.Name);

        var webhookAddress = @$"{client.Options.Host}/{Route}/{client.Options.Token}";

        if (TelegramWebHookOptions.DeleteOldHooks)
            await client.Client.DeleteWebhookAsync(cancellationToken: client.CancellationTokenSource.Token);
        await client.Client.SetWebhookAsync(
            webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(), cancellationToken: client.CancellationTokenSource.Token);

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

                        await using var scope = _services.CreateAsyncScope();
                        var botsCollections = scope.ServiceProvider.GetRequiredService<BotsCollection<TKey>>();

                        if (tgClient.BotId != null)
                        {
                            var botId = botsCollections.Values[client.Options.Name];
                            var serviceFactory =
                                (IServiceFactory)scope.ServiceProvider.GetRequiredService(typeof(IServiceFactory));
                            var userService = serviceFactory.CreateUserService<TKey, TUser>(botId);
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
}