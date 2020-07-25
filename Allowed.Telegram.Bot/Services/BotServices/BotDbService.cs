using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.StateServices;
using Allowed.Telegram.Bot.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.BotServices
{
    public class BotDbService<TUser, TRole, TState> : BotService
        where TUser : class
        where TRole : class
        where TState : class
    {
        public BotDbService(IServiceProvider services)
            : base(services)
        { }

        private IUserService<TUser> GetUserService()
        {
            return (IUserService<TUser>)
                Services.GetService(typeof(IUserService<TUser>));
        }

        private IRoleService<TRole> GetRoleService()
        {
            return (IRoleService<TRole>)
                Services.GetService(typeof(IRoleService<TRole>));
        }

        private IStateService<TState> GetStateService()
        {
            return (IStateService<TState>)
                Services.GetService(typeof(IStateService<TState>));
        }

        private IMessageHandler GetMessageHandler(IRoleService<TRole> roleService, IStateService<TState> stateService,
            ITelegramBotClient client, BotData botData)
        {
            return new MessageHandler<TRole, TState>(GetControllersCollection(),
                client, botData, roleService, stateService, Services);
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            ClientsCollection clientsCollection = Services.GetService<ClientsCollection>();

            foreach (ClientItem client in clientsCollection.Clients)
            {
                client.Client.OnMessage += (a, b) =>
                {
                    IUserService<TUser> userService = GetUserService();
                    IRoleService<TRole> roleService = GetRoleService();
                    IStateService<TState> stateService = GetStateService();

                    IMessageHandler messageHandler =
                        GetMessageHandler(roleService, stateService, client.Client, client.BotData);

                    userService.CheckUser(b.Message.Chat.Id, b.Message.Chat.Username);
                    messageHandler.OnMessage(a, b);
                };

                client.Client.OnCallbackQuery += (a, b) =>
                {
                    IUserService<TUser> userService = GetUserService();
                    IRoleService<TRole> roleService = GetRoleService();
                    IStateService<TState> stateService = GetStateService();

                    IMessageHandler messageHandler =
                        GetMessageHandler(roleService, stateService, client.Client, client.BotData);

                    userService.CheckUser(b.CallbackQuery.Message.Chat.Id, b.CallbackQuery.Message.Chat.Username);
                    messageHandler.OnCallbackQuery(a, b);
                };

                client.Client.StartReceiving();
            }

            await Task.Delay(Timeout.Infinite);
        }
    }
}
