using System;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Middlewares
{
    public abstract class MessageDbMiddleware<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual Task AfterMessageProcessedAsync(TKey botId, long telegramId) { return Task.CompletedTask; }
        public virtual Task AfterCallbackProcessedAsync(TKey botId, long telegramId) { return Task.CompletedTask; }
        public virtual Task AfterInlineProcessedAsync(TKey botId, long telegramId) { return Task.CompletedTask; }

        public virtual void AfterMessageProcessed(TKey botId, long telegramId) { }
        public virtual void AfterCallbackProcessed(TKey botId, long telegramId) { }
        public virtual void AfterInlineProcessed(TKey botId, long telegramId) { }
    }
}
