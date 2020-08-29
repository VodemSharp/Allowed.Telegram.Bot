using Allowed.Telegram.Bot.Data.Constants;
using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(AllowedConstants.DbConnection),
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddTelegramClients(new BotData[] {
                    new BotData { Token = "1235322308:AAGlWMx1Avo52Hjr3ST22e7XKw577qFwOrg", Name = "Sample" },
                    new BotData { Token = "1289911268:AAE4j2kkt8dPZKKr2MLpGLTP65gvaKA-sDA", Name = "Sample2" },
                })
                .AddTelegramStore<ApplicationDbContext>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseTelegramBots();
        }
    }
}
