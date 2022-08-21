using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Extensions.Collections.Items;
using Allowed.Telegram.Bot.Managers;
using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Extensions;

public static class ClientsExtensions
{
    public static IServiceCollection AddTelegramClients(this IServiceCollection services, IEnumerable<BotData> data)
    {
        services.AddSingleton(_ =>
            new ClientsCollection
            {
                Clients = data
                    .Select(d => new ClientItem { Client = new TelegramBotClient(d.Token), BotData = d }).ToList()
            });

        return services;
    }

    public static IServiceCollection AddTelegramManager(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
            new ControllersCollection
            {
                ControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.IsSubclassOf(typeof(CommandController))).ToList()
            });

        services.AddHostedService<TelegramManager>();

        return services;
    }

    public static IServiceCollection AddTelegramWebHookManager(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
            new ControllersCollection
            {
                ControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.IsSubclassOf(typeof(CommandController))).ToList()
            });

        services.AddHostedService<TelegramWebHookManager>();

        return services;
    }
}