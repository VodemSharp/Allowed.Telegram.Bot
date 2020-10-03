using Allowed.Telegram.Bot.Factories.ServiceFactories;
using System;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Controllers
{
    public abstract class CommandController<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey BotId { get; set; }

        public virtual void Initialize(IServiceFactory factory, long chatId) { }
        public virtual Task InitializeAsync(IServiceFactory factory, long chatId) { return Task.CompletedTask; }
    }
}
