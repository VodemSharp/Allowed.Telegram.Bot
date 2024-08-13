using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Execution.Updates;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot;

public static class TelegramClientsExtensions
{
    public static IServiceCollection AddTelegramServices(this IServiceCollection services)
    {
        services.AddSingleton<CommandActionGlobalCollection>();
        services.AddSingleton<TelegramHandlerList>();
        services.AddSingleton(typeof(ICommandCollection<>), typeof(CommandCollection<>));

        services.AddTransient<MessageCommandHandler>();
        services.AddTransient<UpdateCommandHandler>();

        return services;
    }

    public static IServiceCollection AddTelegramManager(this IServiceCollection services)
    {
        return services.AddSingleton<ITelegramManager, TelegramManager>();
    }

    // TODO
    // public static IServiceCollection AddTelegramWebHookManager(this IServiceCollection services,
    //     Action<TelegramWebHookOptions>? configureOptions = null)
    // {
    //     if (configureOptions != null) services.Configure(configureOptions);
    //     return services.AddTransient<ITelegramManager, TelegramWebHookManager>();
    // }
}