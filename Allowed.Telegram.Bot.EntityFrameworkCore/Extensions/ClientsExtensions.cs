using Allowed.Telegram.Bot.Abstractions;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Data.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions.Items;
using Allowed.Telegram.Bot.EntityFrameworkCore.Managers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;

public static class ClientsExtensions
{
    public static IServiceCollection AddTelegramDbControllers(this IServiceCollection services)
    {
        services.AddSingleton(typeof(BotsCollection<>));

        return services.AddSingleton(_ =>
            new ControllersCollection
            {
                ControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => TypeHelper.IsTypeDerivedFromGenericType(p, typeof(CommandController<>)))
                    .ToList()
            });
    }

    public static IServiceCollection AddTelegramDbManager(this IServiceCollection services)
    {
        services.AddSingleton<ClientsCollection>();
        
        services.AddSingleton<ITelegramManager>(provider =>
        {
            var options = provider.GetRequiredService<ContextOptions>();

            return (TelegramManager)ActivatorUtilities.CreateInstance(provider,
                typeof(TelegramDbManager<,,,>).MakeGenericType(options.KeyType, options.UserType,
                    options.RoleType, options.BotType));
        });

        return services;
    }

    public static IServiceCollection AddTelegramDbWebHookManager(this IServiceCollection services)
    {
        services.AddSingleton<ClientsCollection>();
        
        services.AddSingleton<ITelegramManager>(provider =>
        {
            var options = provider.GetRequiredService<ContextOptions>();

            return (TelegramWebHookManager)ActivatorUtilities.CreateInstance(provider,
                typeof(TelegramDbWebHookManager<,,,>).MakeGenericType(options.KeyType, options.UserType,
                    options.RoleType, options.BotType));
        });

        return services;
    }
}