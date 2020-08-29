using Allowed.Telegram.Bot.Data.Constants;
using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Allowed.Telegram.Bot.Sample
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(AllowedConstants.DbConnection),
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            //services.AddTelegramClients(new BotData[] {
            //        //new BotData { Token = "<token>", Name = "Sample" },
            //        //new BotData { Token = "<token2>", Name = "Sample2" },
            //    })
            //    .AddTelegramStore<ApplicationDbContext>();

            services.AddTelegramClients(Configuration.GetSection("Telegram:Bots").Get<BotData[]>())
                .AddTelegramStore<ApplicationDbContext>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseTelegramBots();
        }
    }
}
