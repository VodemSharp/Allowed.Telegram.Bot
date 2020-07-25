using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Options;
using Allowed.Telegram.Bot.Services.BotServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Extensions
{
    public static class ClientsExtensions
    {
        public static IServiceCollection AddTelegramClients(this IServiceCollection services, IEnumerable<BotData> data)
        {
            services.AddSingleton((servs) =>
                new ClientsCollection
                {
                    Clients = data
                        .Select(d => new ClientItem { Client = new TelegramBotClient(d.Token), BotData = d }).ToList()
                });

            services.AddSingleton((IServiceProvider provider) =>
            {
                ContextOptions options = provider.GetService<ContextOptions>();

                if (options == null)
                    return ActivatorUtilities.CreateInstance<BotService>(provider);

                return (BotService)ActivatorUtilities.CreateInstance(provider,
                    typeof(BotDbService<,,>).MakeGenericType(new Type[] {
                        options.UserType, options.RoleType, options.StateType
                    }));
            });

            return services;
        }

        public static IApplicationBuilder UseTelegramBots(this IApplicationBuilder app)
        {
            IServiceProvider provider = app.ApplicationServices;

            provider.GetService<BotService>().StartAsync(new System.Threading.CancellationToken());

            return app;
        }
    }
}
