using Allowed.Telegram.Bot.Builders;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Factories.ServiceFactories;
using Allowed.Telegram.Bot.Handlers.MessageHandlers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Options;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.StateServices;
using Allowed.Telegram.Bot.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.BotServices
{
    public class BotDbService<TKey, TUser, TBotUser, TRole, TBot, TState> : BotService
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
        where TBotUser : TelegramBotUser<TKey>
        where TRole : TelegramRole<TKey>
        where TBot : TelegramBot<TKey>
        where TState : TelegramState<TKey>
    {
        private readonly IServiceFactory _serviceFactory;

        public BotDbService(IServiceProvider services,
            ControllersCollection controllersCollection)
            : base(services, controllersCollection)
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

        private IStateService<TKey, TState> GetStateService(TKey botId)
        {
            return _serviceFactory.CreateStateService<TKey, TState>(botId);
        }

        private MessageDbHandler<TKey, TRole, TState> GetMessageHandler(
            IRoleService<TKey, TRole> roleService, IStateService<TKey, TState> stateService,
            ITelegramBotClient client, BotData botData)
        {
            return new MessageDbHandler<TKey, TRole, TState>(_controllersCollection,
                client, botData, roleService, stateService, Services);
        }

        private async Task<Dictionary<string, TKey>> InitializeBots(IEnumerable<string> botNames)
        {
            ContextOptions options = Services.GetService<ContextOptions>();
            Dictionary<string, TKey> result = new Dictionary<string, TKey> { };

            using (DbContext db = (DbContext)Services.GetService(options.ContextType))
            {
                foreach (string botName in botNames)
                {
                    if (!await db.Set<TBot>().AnyAsync(b => b.Name == botName))
                    {
                        await db.Set<TBot>().AddAsync(ContextBuilder.CreateTelegramBot<TKey, TBot>(botName));
                        await db.SaveChangesAsync();
                    }

                    result.Add(botName, (await db.Set<TBot>()
                        .FirstOrDefaultAsync(b => b.Name == botName)).Id);
                }
            }

            return result;
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            ClientsCollection clientsCollection = Services.GetService<ClientsCollection>();
            Dictionary<string, TKey> bots =
                await InitializeBots(clientsCollection.Clients.Select(c => c.BotData.Name));

            foreach (ClientItem client in clientsCollection.Clients)
            {
                TKey botId = bots.GetValueOrDefault(client.BotData.Name);

                client.Client.OnMessage += async (a, b) =>
                {
                    IUserService<TKey, TUser> userService = GetUserService(botId);
                    IRoleService<TKey, TRole> roleService = GetRoleService(botId);
                    IStateService<TKey, TState> stateService = GetStateService(botId);

                    MessageDbHandler<TKey, TRole, TState> messageHandler =
                        GetMessageHandler(roleService, stateService, client.Client, client.BotData);

                    await userService.CheckUser(b.Message.Chat.Id, b.Message.Chat.Username);
                    await messageHandler.OnMessage(b, botId);
                };

                client.Client.OnCallbackQuery += async (a, b) =>
                {
                    IUserService<TKey, TUser> userService = GetUserService(botId);
                    IRoleService<TKey, TRole> roleService = GetRoleService(botId);
                    IStateService<TKey, TState> stateService = GetStateService(botId);

                    MessageDbHandler<TKey, TRole, TState> messageHandler =
                        GetMessageHandler(roleService, stateService, client.Client, client.BotData);

                    await userService.CheckUser(b.CallbackQuery.Message.Chat.Id, b.CallbackQuery.Message.Chat.Username);
                    await messageHandler.OnCallbackQuery(a, b);
                };

                client.Client.StartReceiving();
            }

            await Task.Delay(Timeout.Infinite);
        }
    }
}
