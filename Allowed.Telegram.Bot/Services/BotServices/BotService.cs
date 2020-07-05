using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Options;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
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

        protected List<CommandController> GetCommandControllers()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(typeof(CommandController)))
                        .Select(t => (CommandController)ActivatorUtilities.CreateInstance(Services, t)).ToList();
        }

        protected IControllersCollection GetControllersCollection()
        {
            return new ControllersCollection
            {
                Controllers = GetCommandControllers()
            };
        }

        protected IMessageHandler GetMessageHandler(ITelegramBotClient client, BotData botData)
        {
            return new MessageHandler<object, object>(GetControllersCollection(), client, botData, null, null);
        }

        protected virtual async Task DoWork(CancellationToken stoppingToken)
        {
            IClientsCollection clientsCollection = Services.GetService<IClientsCollection>();
            ContextOptions options = Services.GetService<ContextOptions>();

            foreach (ClientItem client in clientsCollection.Clients)
            {
                IMessageHandler messageHandler = GetMessageHandler(client.Client, client.BotData);

                client.Client.OnMessage += (a, b) => messageHandler.OnMessage(a, b);
                client.Client.OnCallbackQuery += (a, b) => messageHandler.OnCallbackQuery(a, b);

                client.Client.StartReceiving();
            }

            await Task.Delay(Timeout.Infinite);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}
