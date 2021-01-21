using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.Sample.NoDb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTelegramClients(Configuration.GetSection("Telegram:Bots").Get<BotData[]>())
                    .AddTelegramManager();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseTelegramBots();
        }
    }
}
