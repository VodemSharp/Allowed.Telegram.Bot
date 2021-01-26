using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Data.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Managers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Managers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions
{
    public static class ClientsExtensions
    {
        public static IServiceCollection AddTelegramDbManager(this IServiceCollection services)
        {
            services.AddSingleton((servs) =>
                new ControllersCollection
                {
                    ControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p =>
                        {
                            return TypeHelper.IsTypeDerivedFromGenericType(p, typeof(CommandController<>));
                        }).ToList()
                });

            services.AddHostedService((IServiceProvider provider) =>
            {
                ContextOptions options = provider.GetService<ContextOptions>();

                return (TelegramManager)ActivatorUtilities.CreateInstance(provider,
                    typeof(TelegramDbManager<,,,,,>).MakeGenericType(new Type[] {
                        options.KeyType, options.UserType, options.BotUserType, options.RoleType, options.BotType, options.StateType
                }));
            });

            return services;
        }
    }
}
