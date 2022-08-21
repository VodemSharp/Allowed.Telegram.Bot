using Microsoft.EntityFrameworkCore;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions;

public static class DbContextExtensions
{
    public static IQueryable Set(this DbContext context, Type t)
    {
        var set = context.GetType().GetMethods().Single(m => m.Name == "Set" && m.GetParameters().Length == 0);
        return (IQueryable)set.MakeGenericMethod(t).Invoke(context, null);
    }
}