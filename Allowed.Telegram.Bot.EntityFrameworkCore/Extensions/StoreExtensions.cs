using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.EntityFrameworkCore.Factories;
using Allowed.Telegram.Bot.EntityFrameworkCore.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;

public static class StoreExtensions
{
    private static Func<Type, bool> GetSetTypeCheck(Type checkedType)
    {
        return t => t.BaseType is { IsGenericType: true } &&
                    t.BaseType.GetGenericTypeDefinition() == checkedType;
    }

    public static IServiceCollection AddTelegramStore<TContext>(this IServiceCollection services)
        where TContext : class
    {
        var contextType = typeof(TContext);

        var dbSet = typeof(DbSet<>);
        var setTypes = contextType.GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == dbSet)
            .Select(p => p.PropertyType.GenericTypeArguments[0]).ToList();

        var userType = typeof(TelegramUser<>);
        var roleType = typeof(TelegramRole<>);
        var botType = typeof(TelegramBot<>);
        var botUserType = typeof(TelegramBotUser<>);
        var botUserRoleType = typeof(TelegramBotUserRole<>);

        ContextOptions options = new()
        {
            ContextType = contextType,

            UserType = setTypes.Single(GetSetTypeCheck(userType)),
            RoleType = setTypes.Single(GetSetTypeCheck(roleType)),
            BotType = setTypes.Single(GetSetTypeCheck(botType)),
            BotUserType = setTypes.Single(GetSetTypeCheck(botUserType)),
            BotUserRoleType = setTypes.Single(GetSetTypeCheck(botUserRoleType))
        };

        options.KeyType = options.UserType.BaseType!.GenericTypeArguments[0];

        services.AddSingleton(options);
        services.AddTransient<IServiceFactory, ServiceFactory>();

        return services;
    }
}