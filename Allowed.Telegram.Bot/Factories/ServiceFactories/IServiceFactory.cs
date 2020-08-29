using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.StateServices;
using Allowed.Telegram.Bot.Services.UserServices;
using System;

namespace Allowed.Telegram.Bot.Factories.ServiceFactories
{
    public interface IServiceFactory
    {
        IUserService<TKey, TUser> CreateUserService<TKey, TUser>(TKey botId)
           where TKey : IEquatable<TKey>
           where TUser : TelegramUser<TKey>;

        public IRoleService<TKey, TRole> CreateRoleService<TKey, TRole>(TKey botId)
           where TKey : IEquatable<TKey>
           where TRole : TelegramRole<TKey>;

        IStateService<TKey, TState> CreateStateService<TKey, TState>(TKey botId)
           where TKey : IEquatable<TKey>
           where TState : TelegramState<TKey>;
    }
}
