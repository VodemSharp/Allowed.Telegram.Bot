using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Microsoft.Extensions.Hosting;
using System;
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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DoWork(stoppingToken);

            return Task.CompletedTask;
        }

        private void DoWork(CancellationToken stoppingToken)
        {
            ITelegramBotClient botClient =
                (ITelegramBotClient)Services.GetService(typeof(ITelegramBotClient));

            botClient.OnMessage += (a, b) =>
            {
                ((IMessageHandler)Services.GetService(typeof(IMessageHandler)))
                    .OnMessage(a, b);
            };

            botClient.OnCallbackQuery += (a, b) =>
            {
                ((IMessageHandler)Services.GetService(typeof(IMessageHandler)))
                    .OnCallbackQuery(a, b);
            };

            botClient.StartReceiving();
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }
    }
}
