using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
using Allowed.Telegram.Bot.Services.TelegramServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Services
{
    public class BotService : BackgroundService
    {
        public BotService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DoWork(stoppingToken);

            return Task.CompletedTask;
        }

        private List<CommandController> GetCommandControllers()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(typeof(CommandController)))
                        .Select(t => (CommandController)ActivatorUtilities.CreateInstance(Services, t)).ToList();
        }

        private void DoWork(CancellationToken stoppingToken)
        {
            IClientsCollection clientsCollection = (IClientsCollection)Services.GetService(typeof(IClientsCollection));
            ITelegramService telegramService = (ITelegramService)Services.GetService(typeof(ITelegramService));

            IControllersCollection controllersCollection = new ControllersCollection
            {
                Controllers = GetCommandControllers()
            };

            foreach (ClientItem client in clientsCollection.Clients)
            {
                IMessageHandler messageHandler = new MessageHandler(controllersCollection, client.Client, client.BotData,
                                                                    telegramService);

                if (telegramService != null)
                {
                    client.Client.OnMessage += (a, b) =>
                    {
                        telegramService.CheckUser(b.Message.Chat.Id, b.Message.Chat.Username);
                        messageHandler.OnMessage(a, b);
                    };

                    client.Client.OnCallbackQuery += (a, b) =>
                    {
                        telegramService.CheckUser(b.CallbackQuery.Message.Chat.Id, b.CallbackQuery.Message.Chat.Username);
                        messageHandler.OnCallbackQuery(a, b);
                    };
                }
                else
                {
                    client.Client.OnMessage += (a, b) => messageHandler.OnMessage(a, b);
                    client.Client.OnCallbackQuery += (a, b) => messageHandler.OnCallbackQuery(a, b);
                }

                client.Client.StartReceiving();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}
