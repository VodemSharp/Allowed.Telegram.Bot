using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Middlewares
{
    public abstract class MessageMiddleware
    {
        public Task AfterMessageProcessedAsync(long telegramId) { return Task.CompletedTask; }
        public Task AfterCallbackProcessedAsync(long telegramId) { return Task.CompletedTask; }
        public Task AfterInlineProcessedAsync(long telegramId) { return Task.CompletedTask; }

        public void AfterMessageProcessed(long telegramId) { }
        public void AfterCallbackProcessed(long telegramId) { }
        public void AfterInlineProcessed(long telegramId) { }
    }
}
