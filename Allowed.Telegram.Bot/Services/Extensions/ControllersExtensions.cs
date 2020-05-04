using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Handlers.MessageHandler;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Services.Extensions
{
    public static class ControllersExtensions
    {
        public static IServiceCollection AddTelegramControllers(this IServiceCollection services, string token)
        {
            ITelegramBotClient client = new TelegramBotClient(token);
            services.AddSingleton(client);

            Type type = typeof(CommandController);

            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsSubclassOf(type)).ToList();

            services.AddSingleton<IControllersCollection>(
                new ControllersCollection
                {
                    Controllers = types.Select(t =>
                    {
                        CommandController controller = (CommandController)Activator.CreateInstance(t);
                        controller.Client = client;
                        return controller;

                    }).ToList()
                });

            services.AddTransient<IMessageHandler, MessageHandler>();

            return services;
        }
    }
}
