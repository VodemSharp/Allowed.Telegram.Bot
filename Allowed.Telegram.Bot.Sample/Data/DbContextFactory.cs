using Allowed.Telegram.Bot.Data.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Allowed.Telegram.Bot.Sample.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            dbContextBuilder.UseMySql(AllowedConstants.DbConnection);

            return new ApplicationDbContext(dbContextBuilder.Options);
        }
    }
}
