using System.Net;
using System.Text.Json;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Handlers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Managers;
using Allowed.Telegram.Bot.Models;
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
    private readonly IServiceFactory _serviceFactory;

    public TelegramDbManager(IServiceProvider services,
        ControllersCollection controllersCollection,
        ILoggerFactory loggerFactory)
        : base(services, controllersCollection, loggerFactory)
    {
        _serviceFactory = (IServiceFactory)services.GetService(typeof(IServiceFactory));
    }

    private IUserService<TKey, TUser> GetUserService(TKey botId)
    {
        return _serviceFactory.CreateUserService<TKey, TUser>(botId);
    }

    private IRoleService<TKey, TRole> GetRoleService(TKey botId)
    {
        return _serviceFactory.CreateRoleService<TKey, TRole>(botId);
    }

    private MessageDbHandler<TKey, TUser, TRole> GetMessageHandler(
        IUserService<TKey, TUser> userService,
        IRoleService<TKey, TRole> roleService,
        ITelegramBotClient client, SimpleTelegramBotClientOptions options)
    {
        return new MessageDbHandler<TKey, TUser, TRole>(ControllersCollection,
            client, options, userService, roleService, Services);
    }

    private async Task<Dictionary<string, TKey>> InitializeBots(IEnumerable<string> botNames)
    {
        var options = Services.GetRequiredService<ContextOptions>();
        var result = new Dictionary<string, TKey>();

        await using var db = (DbContext)Services.GetRequiredService(options.ContextType);
        foreach (var botName in botNames)
        {
            if (!await db.Set<TBot>().AnyAsync(b => b.Name == botName))
            {
                await db.Set<TBot>().AddAsync(ContextBuilder.CreateTelegramBot<TKey, TBot>(botName));
                await db.SaveChangesAsync();
            }

            result.Add(botName, (await db.Set<TBot>().OrderBy(b => b.Id).SingleAsync(b => b.Name == botName)).Id);
        }

        return result;
    }

    protected override async Task DoWork(CancellationToken stoppingToken)
    {
        try
        {
            var clientsCollection = Services.GetRequiredService<ClientsCollection>();
            var bots =
                await InitializeBots(clientsCollection.Clients.Select(c => c.Options.Name));

            foreach (var client in clientsCollection.Clients)
            {
                var botId = bots.GetValueOrDefault(client.Options.Name);

                var userService = GetUserService(botId);
                var roleService = GetRoleService(botId);

                var messageHandler =
                    GetMessageHandler(userService, roleService, client.Client, client.Options);

                async void UpdateHandler(ITelegramBotClient tgClient, Update update, CancellationToken token)
                {
                    await messageHandler.OnUpdate(tgClient, update, botId, token);
                }

                var receiverOptions = new ReceiverOptions();

                await client.Client.DeleteWebhookAsync(cancellationToken: stoppingToken);
                client.Client.StartReceiving(UpdateHandler,
                    (tgClient, exception, _) => Logger.LogError("{botId}:\n{exception}",
                        tgClient.BotId, exception.ToString()),
                    receiverOptions, stoppingToken);

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

                                await GetUserService(botId).BlockBot(telegramId);
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