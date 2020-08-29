using Allowed.Telegram.Bot.Controllers;
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
        private static bool IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
        {
            if (typeToCheck == typeof(object) || typeToCheck == null)
            {
                return false;
            }
            else if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        }

        public static IServiceCollection AddTelegramClients(this IServiceCollection services, IEnumerable<BotData> data)
        {
            services.AddSingleton((servs) =>
                new ClientsCollection
                {
                    Clients = data
                        .Select(d => new ClientItem { Client = new TelegramBotClient(d.Token), BotData = d }).ToList()
                });


            services.AddSingleton((servs) =>
                new ControllersCollection
                {
                    ControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p =>
                        {
                            return IsTypeDerivedFromGenericType(p, typeof(CommandController<>));
                        }).ToList()
                });

            services.AddSingleton((IServiceProvider provider) =>
            {
                ContextOptions options = provider.GetService<ContextOptions>();

                if (options == null)
                    return ActivatorUtilities.CreateInstance<BotService>(provider);

                return (BotService)ActivatorUtilities.CreateInstance(provider,
                    typeof(BotDbService<,,,,,>).MakeGenericType(new Type[] {
                        options.KeyType, options.UserType, options.BotUserType, options.RoleType, options.BotType, options.StateType
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
