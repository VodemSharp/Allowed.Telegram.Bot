using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.StateServices;
using Allowed.Telegram.Bot.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Allowed.Telegram.Bot.Factories.ServiceFactories
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _provider;

        public ServiceFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IUserService<TKey, TUser> CreateUserService<TKey, TUser>(TKey botId)
            where TKey : IEquatable<TKey>
            where TUser : TelegramUser<TKey>
        {
            return (IUserService<TKey, TUser>)ActivatorUtilities.CreateInstance(
                _provider, typeof(UserService<TKey, TUser>), botId);
        }

        public IRoleService<TKey, TRole> CreateRoleService<TKey, TRole>(TKey botId)
           where TKey : IEquatable<TKey>
           where TRole : TelegramRole<TKey>
        {
            return (IRoleService<TKey, TRole>)ActivatorUtilities.CreateInstance(
                _provider, typeof(RoleService<TKey, TRole>), botId);
        }

        public IStateService<TKey, TState> CreateStateService<TKey, TState>(TKey botId)
           where TKey : IEquatable<TKey>
           where TState : TelegramState<TKey>
        {
            return (IStateService<TKey, TState>)ActivatorUtilities.CreateInstance(
                _provider, typeof(StateService<TKey, TState>), botId);
        }
    }
}
