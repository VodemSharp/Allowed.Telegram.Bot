using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions
{
    public static partial class DbContextExtensions
    {
        public static IQueryable Set(this DbContext context, Type t)
        {
            MethodInfo set = context.GetType().GetMethods().First(m => m.Name == "Set" && m.GetParameters().Length == 0);
            return (IQueryable)set.MakeGenericMethod(t).Invoke(context, null);
        }
    }
}
