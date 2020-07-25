using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.Sample.NoDb
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTelegramClients(new BotData[] {
                    new BotData { Token = "1235322308:AAGlWMx1Avo52Hjr3ST22e7XKw577qFwOrg", Name = "Sample" },
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseTelegramBots();
        }
    }
}
