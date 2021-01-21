using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Contexts;
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
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)),
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddTelegramClients(Configuration.GetSection("Telegram:Bots").Get<BotData[]>())
                .AddTelegramStore<ApplicationDbContext>()
                .AddTelegramDbManager();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseTelegramBots();
        }
    }
}
