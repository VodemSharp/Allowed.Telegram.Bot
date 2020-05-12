using Allowed.Telegram.Bot.Models.Store.Context;
using Allowed.Telegram.Bot.Services.TelegramServices;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.Services.Extensions
{
    public static class StoreExtensions
    {
        public static IServiceCollection AddTelegramStore<T>(this IServiceCollection services) where T: AllowedTelegramBotDbContext
        {
            services.AddSingleton<AllowedTelegramBotDbContext, T>();
            services.AddSingleton<ITelegramService, TelegramService>();

            return services;
        }
    }
}
