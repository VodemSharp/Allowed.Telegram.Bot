using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
using Microsoft.Extensions.Hosting;
using System;
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

        private void DoWork(CancellationToken stoppingToken)
        {
            IControllersCollection controllersCollection = (IControllersCollection)Services.GetService(typeof(IControllersCollection));
            IClientsCollection clientsCollection = (IClientsCollection)Services.GetService(typeof(IClientsCollection));

            foreach (ClientItem client in clientsCollection.Clients)
            {
                IMessageHandler messageHandler = new MessageHandler(controllersCollection, client.Client, client.Name);

                client.Client.OnMessage += (a, b) => messageHandler.OnMessage(a, b);
                client.Client.OnCallbackQuery += (a, b) => messageHandler.OnCallbackQuery(a, b);

                client.Client.StartReceiving();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}
