using Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;
using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Sample.NoDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration config = hostContext.Configuration;

                    string connection = config.GetConnectionString("DefaultConnection");
                    services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)),
                        ServiceLifetime.Transient, ServiceLifetime.Transient);

                    services.AddTelegramClients(config.GetSection("Telegram:Bots").Get<BotData[]>())
                        .AddTelegramStore<ApplicationDbContext>()
                        .AddTelegramDbManager();
                });
    }
}
