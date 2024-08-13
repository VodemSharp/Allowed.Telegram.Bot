using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.Extensions;

public static class ClientsExtensions
{
    public static IServiceCollection AddTelegramControllers(this IServiceCollection services)
    {
        return services.AddSingleton(_ =>
            new ControllersCollection
            {
                ControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.IsSubclassOf(typeof(CommandController))).ToList()
            });
    }

    public static IServiceCollection AddTelegramManager(this IServiceCollection services)
    {
        services.AddSingleton<ClientsCollection>();
        return services.AddSingleton<ITelegramManager, TelegramManager>();
    }

    public static IServiceCollection AddTelegramWebHookManager(this IServiceCollection services)
    {
        services.AddSingleton<ClientsCollection>();
        return services.AddTransient<ITelegramManager, TelegramWebHookManager>();
    }
}