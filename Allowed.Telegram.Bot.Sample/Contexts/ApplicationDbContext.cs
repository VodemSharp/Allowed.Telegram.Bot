using Allowed.Telegram.Bot.EntityFrameworkCore.Contexts;
using Allowed.Telegram.Bot.Sample.DbModels;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace Allowed.Telegram.Bot.Sample.Contexts
{
    public class ApplicationDbContext :
        TelegramBotDbContext<int, ApplicationTgUser, ApplicationTgRole, ApplicationTgBot, ApplicationTgBotUser, ApplicationTgBotUserRole, ApplicationTgState>
    {
        public DbSet<UserFile> UserFiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // UserFile
            builder.Entity<UserFile>().HasKey(uf => uf.Id);

            builder.Entity<UserFile>().HasOne(uf => uf.TelegramUser)
                                      .WithMany(uf => uf.UserFiles)
                                      .HasForeignKey(uf => uf.TelegramUserId);
        }
        
        public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{envName}.json", optional: false)
                    .Build();

                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                string connection = config.GetConnectionString("DefaultConnection");

                builder.UseNpgsql(connection);

                return new ApplicationDbContext(builder.Options);
            }
        }
    }
}
