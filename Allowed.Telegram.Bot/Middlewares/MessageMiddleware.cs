using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Middlewares
{
    public abstract class MessageMiddleware
    {
        public Task AfterMessageProcessedAsync(long chatId) { return Task.CompletedTask; }
        public Task AfterCallbackProcessedAsync(long chatId) { return Task.CompletedTask; }
        public Task AfterInlineProcessedAsync(long chatId) { return Task.CompletedTask; }

        public void AfterMessageProcessed(long chatId) { }
        public void AfterCallbackProcessed(long chatId) { }
        public void AfterInlineProcessed(long chatId) { }
    }
}
