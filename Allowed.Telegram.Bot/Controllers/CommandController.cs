using Allowed.Telegram.Bot.Factories.ServiceFactories;
using System;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Controllers
{
    public abstract class CommandController<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey BotId { get; set; }

        public virtual void Initialize(IServiceFactory factory) { }
        public virtual Task InitializeAsync(IServiceFactory factory) { return Task.CompletedTask; }
    }
}
