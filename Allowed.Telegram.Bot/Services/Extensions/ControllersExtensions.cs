using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.Extensions
{
    public static class ControllersExtensions
    {
        public static IServiceCollection AddTelegramControllers(this IServiceCollection services, IEnumerable<BotData> data)
        {
            services.AddCommandControllers();
            services.AddTelegramClients(data);

            services.AddHostedService<BotService>();

            return services;
        }

        private static void AddCommandControllers(this IServiceCollection services)
        {
            services.TryAddSingleton<IControllersCollection>(
                new ControllersCollection
                {
                    Controllers = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsSubclassOf(typeof(CommandController)))
                        .Select(t => (CommandController)Activator.CreateInstance(t)).ToList()
                });
        }

        private static void AddTelegramClients(this IServiceCollection services, IEnumerable<BotData> data)
        {
            services.TryAddSingleton<IClientsCollection>(
                new ClientsCollection
                {
                    Clients = data
                        .Select(d => new ClientItem { Client = new TelegramBotClient(d.Token), Name = d.Name }).ToList()
                });
        }
    }
}
