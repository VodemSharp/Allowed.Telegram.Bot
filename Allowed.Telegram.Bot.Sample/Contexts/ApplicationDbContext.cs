using Allowed.Telegram.Bot.EntityFrameworkCore.Contexts;
using Allowed.Telegram.Bot.Sample.DbModels;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Allowed.Telegram.Bot.Sample.Contexts;

public class ApplicationDbContext :
    TelegramBotDbContext<int, ApplicationTgUser, ApplicationTgRole, ApplicationTgBot, ApplicationTgBotUser,
        ApplicationTgBotUserRole>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserFile> UserFiles { get; set; }

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
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{envName}.json", false)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connection = config.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connection);

            return new ApplicationDbContext(builder.Options);
        }
    }
}