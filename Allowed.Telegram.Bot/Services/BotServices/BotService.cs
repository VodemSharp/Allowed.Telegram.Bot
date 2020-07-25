using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.BotServices
{
    public class BotService : BackgroundService
    {
        public BotService(IServiceProvider services)
        {
            Services = services;
        }

        protected IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        protected List<Type> GetCommandControllerTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(typeof(CommandController))).ToList();
        }

        protected ControllersCollection GetControllersCollection()
        {
            return new ControllersCollection
            {
                ControllerTypes = GetCommandControllerTypes()
            };
        }

        protected IMessageHandler GetMessageHandler(ITelegramBotClient client, BotData botData)
        {
            return new MessageHandler<object, object>(GetControllersCollection(),
                client, botData, null, null, Services);
        }

        protected virtual async Task DoWork(CancellationToken stoppingToken)
        {
            ClientsCollection clientsCollection = Services.GetService<ClientsCollection>();

            foreach (ClientItem client in clientsCollection.Clients)
            {
                IMessageHandler messageHandler = GetMessageHandler(client.Client, client.BotData);

                client.Client.OnMessage += (a, b) => messageHandler.OnMessage(a, b);
                client.Client.OnCallbackQuery += (a, b) => messageHandler.OnCallbackQuery(a, b);

                client.Client.StartReceiving();
            }

            await Task.Delay(Timeout.Infinite);
        }
    }
}
