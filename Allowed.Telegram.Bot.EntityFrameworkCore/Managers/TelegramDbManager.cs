﻿using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Builders;
using Allowed.Telegram.Bot.EntityFrameworkCore.Handlers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Managers;
using Allowed.Telegram.Bot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Managers
{
    public class TelegramDbManager<TKey, TUser, TBotUser, TRole, TBot> : TelegramManager
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
        where TBotUser : TelegramBotUser<TKey>
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
            ITelegramBotClient client, BotData botData)
        {
            return new MessageDbHandler<TKey, TUser, TRole>(_controllersCollection,
                client, botData, userService, roleService, Services);
        }

        private async Task<Dictionary<string, TKey>> InitializeBots(IEnumerable<string> botNames)
        {
            ContextOptions options = Services.GetService<ContextOptions>();
            Dictionary<string, TKey> result = new() { };

            using (DbContext db = (DbContext)Services.GetService(options.ContextType))
            {
                foreach (string botName in botNames)
                {
                    if (!await db.Set<TBot>().AnyAsync(b => b.Name == botName))
                    {
                        await db.Set<TBot>().AddAsync(ContextBuilder.CreateTelegramBot<TKey, TBot>(botName));
                        await db.SaveChangesAsync();
                    }

                    result.Add(botName, (await db.Set<TBot>().OrderBy(b => b.Id)
                        .FirstOrDefaultAsync(b => b.Name == botName)).Id);
                }
            }

            return result;
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                ClientsCollection clientsCollection = Services.GetService<ClientsCollection>();
                Dictionary<string, TKey> bots =
                    await InitializeBots(clientsCollection.Clients.Select(c => c.BotData.Name));

                foreach (ClientItem client in clientsCollection.Clients)
                {
                    TKey botId = bots.GetValueOrDefault(client.BotData.Name);

                    client.Client.OnMessage += async (a, b) =>
                    {
                        try
                        {
                            IUserService<TKey, TUser> userService = GetUserService(botId);
                            IRoleService<TKey, TRole> roleService = GetRoleService(botId);

                            MessageDbHandler<TKey, TUser, TRole> messageHandler =
                                GetMessageHandler(userService, roleService, client.Client, client.BotData);

                            await userService.CheckUser(b.Message.From);
                            await messageHandler.OnMessage(b, botId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    };

                    client.Client.OnCallbackQuery += async (a, b) =>
                    {
                        try
                        {
                            IUserService<TKey, TUser> userService = GetUserService(botId);
                            IRoleService<TKey, TRole> roleService = GetRoleService(botId);

                            MessageDbHandler<TKey, TUser, TRole> messageHandler =
                                GetMessageHandler(userService, roleService, client.Client, client.BotData);

                            await userService.CheckUser(b.CallbackQuery.From);
                            await messageHandler.OnCallbackQuery(b, botId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    };

                    client.Client.OnInlineQuery += async (a, b) =>
                    {
                        try
                        {
                            IUserService<TKey, TUser> userService = GetUserService(botId);
                            IRoleService<TKey, TRole> roleService = GetRoleService(botId);

                            MessageDbHandler<TKey, TUser, TRole> messageHandler =
                                GetMessageHandler(userService, roleService, client.Client, client.BotData);

                            await userService.CheckUser(b.InlineQuery.From);
                            await messageHandler.OnInlineQuery(b, botId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    };

                    client.Client.ApiResponseReceived += async (a, b) =>
                    {
                        try
                        {
                            if (b.ResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                            {
                                string response = await b.ResponseMessage.Content.ReadAsStringAsync();

                                if (response == "{\"ok\":false,\"error_code\":403,\"description\":\"Forbidden: bot was blocked by the user\"}")
                                {
                                    string request = await b.ApiRequestEventArgs.HttpContent.ReadAsStringAsync();
                                    long telegramId = JsonSerializer.Deserialize<JsonElement>(request).GetProperty("chat_id").GetInt64();

                                    IUserService<TKey, TUser> userService = GetUserService(botId);
                                    await userService.BlockBot(telegramId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    };

                    client.Client.StartReceiving(cancellationToken: stoppingToken);
                }

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
