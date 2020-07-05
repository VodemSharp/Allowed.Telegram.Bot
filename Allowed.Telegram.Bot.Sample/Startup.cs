using Allowed.Telegram.Bot.Data.Constants;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.Data;
using Allowed.Telegram.Bot.Services.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(AllowedConstants.DbConnection),
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddTelegramControllers(new BotData[] {
                    new BotData { Token = "1235322308:AAGlWMx1Avo52Hjr3ST22e7XKw577qFwOrg", Name = "Sample1" },
                    new BotData { Token = "1289911268:AAE4j2kkt8dPZKKr2MLpGLTP65gvaKA-sDA", Name = "Sample2" },
                })
                .AddTelegramStore<ApplicationDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Allowed Telegram Bot version 2.0.0!");
                });
            });
        }
    }
}
