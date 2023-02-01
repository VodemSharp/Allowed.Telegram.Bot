using System.Net;
using System.Text.Json;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Handlers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Managers;

public class TelegramDbManager<TKey, TUser, TRole, TBot> : TelegramManager
    where TKey : IEquatable<TKey>
    where TUser : TelegramUser<TKey>
    where TRole : TelegramRole<TKey>
    where TBot : TelegramBot<TKey>
{
    public TelegramDbManager(IServiceProvider services,
        ControllersCollection controllersCollection,
        ClientsCollection clientsCollection,
        ILoggerFactory loggerFactory)
        : base(services, controllersCollection, clientsCollection, loggerFactory)
    {
    }

    private async Task<TKey> InitializeBots(string botName)
    {
        await using var scope = Services.CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<ContextOptions>();
        await using var db = (DbContext)scope.ServiceProvider.GetRequiredService(options.ContextType);

        if (!await db.Set<TBot>().AnyAsync(b => b.Name == botName))
        {
            await db.Set<TBot>().AddAsync(ContextBuilder.CreateTelegramBot<TKey, TBot>(botName));
            await db.SaveChangesAsync();
        }

        return await db.Set<TBot>().OrderBy(b => b.Id).Where(b => b.Name == botName).Select(b => b.Id).SingleAsync();
    }

    public override async Task Start(ClientItem client)
    {
        if (ClientsCollection.Clients.Any(c => c.Options.Name == client.Options.Name))
        {
            Logger.LogWarning("Telegram bot has already been started!");
            return;
        }

        ClientsCollection.Clients.Add(client);

        var botId = await InitializeBots(client.Options.Name);

        async void UpdateHandler(ITelegramBotClient tgClient, Update update, CancellationToken token)
        {
            await using var scope = Services.CreateAsyncScope();

            var serviceFactory = (IServiceFactory)scope.ServiceProvider.GetRequiredService(typeof(IServiceFactory));
            var userService = serviceFactory.CreateUserService<TKey, TUser>(botId);
            var roleService = serviceFactory.CreateRoleService<TKey, TRole>(botId);

            await new MessageDbHandler<TKey, TUser, TRole>(ControllersCollection,
                    tgClient, client.Options, userService, roleService, scope.ServiceProvider)
                .OnUpdate(tgClient, update, botId, token);
        }

        var receiverOptions = new ReceiverOptions();

        await client.Client.DeleteWebhookAsync(cancellationToken: client.CancellationTokenSource.Token);
        client.Client.StartReceiving(UpdateHandler,
            (tgClient, exception, _) => Logger.LogError("{botId}:\n{exception}",
                tgClient.BotId, exception.ToString()),
            receiverOptions, client.CancellationTokenSource.Token);

        client.Client.OnApiResponseReceived += async (_, args, cancellationToken) =>
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
                        var serviceFactory =
                            (IServiceFactory)scope.ServiceProvider.GetRequiredService(typeof(IServiceFactory));
                        var userService = serviceFactory.CreateUserService<TKey, TUser>(botId);
                        await userService.BlockBot(telegramId);
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