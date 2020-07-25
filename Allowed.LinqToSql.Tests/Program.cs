using Allowed.Telegram.Bot.Data.Constants;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Sample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Allowed.LinqToSql.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider provider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseMySql(AllowedConstants.DbConnection))
            .BuildServiceProvider();

            ApplicationDbContext db = provider.GetService<ApplicationDbContext>();
            string sqlQuery = string.Empty;

            #region Users

            //sqlQuery = db.TelegramUsers.Where(tu => tu.ChatId == 1).ToSql();

            #endregion

            #region Roles

            //sqlQuery = db.TelegramRoles
            //    .Include(tur => tur.TelegramUserRoles)
            //    .ThenInclude(tur => tur.TelegramUser)
            //    .Where(tur => tur.TelegramUserRoles.Any(tur => tur.TelegramUser.ChatId == 1))
            //    .ToSql();

            //sqlQuery = db.TelegramRoles.Where(tr => tr.Id == 1).ToSql();
            //sqlQuery = db.TelegramRoles.Where(tr => tr.Name == "Name").ToSql();

            #endregion

            #region UserRoles

            //sqlQuery = db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
            //    .Where(tur => tur.TelegramRole.Name == "ROLENAME").Select(tur => tur.TelegramUser).ToSql();

            //sqlQuery = db.TelegramUserRoles.Include(tur => tur.TelegramUser).Include(tur => tur.TelegramRole)
            //    .Where(tur => tur.TelegramRole.Id == 1).Select(tur => tur.TelegramUser).ToSql();

            sqlQuery = db.TelegramUserRoles.Include(tur => tur.TelegramRole).Include(tur => tur.TelegramUser)
                .Where(r => r.TelegramUser.ChatId == 1 && r.TelegramRoleId == 2).ToSql();

            #endregion

            #region States

            //sqlQuery = db.TelegramStates.Where(s => s.TelegramUserId == 1).ToSql();
            //sqlQuery = db.TelegramStates.Include(s => s.TelegramBot)
            //    .Where(s => s.TelegramUserId == 1 && s.TelegramBot.Name == "Name").ToSql();

            #endregion

            Console.WriteLine(sqlQuery);
        }
    }
}
