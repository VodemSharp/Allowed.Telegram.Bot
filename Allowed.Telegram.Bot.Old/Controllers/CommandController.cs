namespace Allowed.Telegram.Bot.Controllers;

public abstract class CommandController
{
    public virtual void Initialize(long telegramId)
    {
    }

    public virtual Task InitializeAsync(long telegramId)
    {
        return Task.CompletedTask;
    }
}