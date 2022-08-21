namespace Allowed.Telegram.Bot.Middlewares;

public abstract class MessageMiddleware
{
    public virtual Task AfterMessageProcessedAsync(long telegramId) => Task.CompletedTask;
    public virtual Task AfterCallbackProcessedAsync(long telegramId) => Task.CompletedTask;
    public virtual Task AfterInlineProcessedAsync(long telegramId) => Task.CompletedTask;

    public virtual void AfterMessageProcessed(long telegramId) { }
    public virtual void AfterCallbackProcessed(long telegramId) { }
    public virtual void AfterInlineProcessed(long telegramId) { }
}