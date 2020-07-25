using Allowed.Telegram.Bot.Options;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.StateServices;
using Allowed.Telegram.Bot.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Allowed.Telegram.Bot.Extensions
{
    public static class StoreExtensions
    {
        public static IServiceCollection AddTelegramStore<TContext>(this IServiceCollection services)
            where TContext : class
        {
            Type contextType = typeof(TContext);

            ContextOptions options = new ContextOptions
            {
                ContextType = contextType,

                UserType = contextType.GetProperty("TelegramUsers").PropertyType.GenericTypeArguments[0],
                RoleType = contextType.GetProperty("TelegramUsers").PropertyType.GenericTypeArguments[0],
                UserRoleType = contextType.GetProperty("TelegramUserRoles").PropertyType.GenericTypeArguments[0],
                StateType = contextType.GetProperty("TelegramStates").PropertyType.GenericTypeArguments[0],
                BotType = contextType.GetProperty("TelegramBots").PropertyType.GenericTypeArguments[0]
            };

            services.AddSingleton(options);

            services.AddTransient(typeof(IRoleService<>), typeof(RoleService<>));
            services.AddTransient(typeof(IUserService<>), typeof(UserService<>));
            services.AddTransient(typeof(IStateService<>), typeof(StateService<>));

            return services;
        }
    }
}
