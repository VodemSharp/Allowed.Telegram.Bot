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
            if (data.Any(da => data.Count(d => d.Token == da.Token) > 1))
                throw new Exception("Token duplicate");
            else if (data.Any(da => data.Count(d => d.Name == da.Name) > 1))
                throw new Exception("Name duplicate");

            services.AddTelegramClients(data);
            services.AddHostedService<BotService>();

            return services;
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
