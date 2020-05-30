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
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services
{
    public class BotService : BackgroundService
    {
        public BotService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        private List<CommandController> GetCommandControllers()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(typeof(CommandController)))
                        .Select(t => (CommandController)ActivatorUtilities.CreateInstance(Services, t)).ToList();
        }

        private IControllersCollection GetControllersCollection()
        {
            return new ControllersCollection
            {
                Controllers = GetCommandControllers()
            };
        }

        private ITelegramService GetTelegramService()
        {
            return (ITelegramService)Services.GetService(typeof(ITelegramService));
        }

        private IMessageHandler GetMessageHandler(ITelegramService telegramService, ITelegramBotClient client, BotData botData)
        {
            return new MessageHandler(GetControllersCollection(), client, botData, telegramService);
        }

        public virtual async Task DoWork(CancellationToken stoppingToken)
        {
            IClientsCollection clientsCollection = (IClientsCollection)Services.GetService(typeof(IClientsCollection));

            foreach (ClientItem client in clientsCollection.Clients)
            {
                if (GetTelegramService() != null)
                {
                    client.Client.OnMessage += (a, b) =>
                    {
                        ITelegramService telegramService = GetTelegramService();
                        IMessageHandler messageHandler = GetMessageHandler(telegramService, client.Client, client.BotData);

                        telegramService.CheckUser(b.Message.Chat.Id, b.Message.Chat.Username);
                        messageHandler.OnMessage(a, b);
                    };

                    client.Client.OnCallbackQuery += (a, b) =>
                    {
                        ITelegramService telegramService = GetTelegramService();
                        IMessageHandler messageHandler = GetMessageHandler(telegramService, client.Client, client.BotData);

                        telegramService.CheckUser(b.CallbackQuery.Message.Chat.Id, b.CallbackQuery.Message.Chat.Username);
                        messageHandler.OnCallbackQuery(a, b);
                    };
                }
                else
                {
                    IMessageHandler messageHandler = GetMessageHandler(null, client.Client, client.BotData);

                    client.Client.OnMessage += (a, b) => messageHandler.OnMessage(a, b);
                    client.Client.OnCallbackQuery += (a, b) => messageHandler.OnCallbackQuery(a, b);
                }

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
