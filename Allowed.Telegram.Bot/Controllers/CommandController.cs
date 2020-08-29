using Allowed.Telegram.Bot.Factories.ServiceFactories;
using System;

namespace Allowed.Telegram.Bot.Controllers
{
    public abstract class CommandController<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey BotId { get; set; }

        public virtual void Initialize(IServiceFactory factory) { }
    }
}
