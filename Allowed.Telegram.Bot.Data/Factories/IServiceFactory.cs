using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;

namespace Allowed.Telegram.Bot.Data.Factories;

public interface IServiceFactory
{
    IUserService<TKey, TUser> CreateUserService<TKey, TUser>(TKey botId)
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>;

    IRoleService<TKey, TRole> CreateRoleService<TKey, TRole>(TKey botId)
        where TKey : IEquatable<TKey>
        where TRole : TelegramRole<TKey>;
}