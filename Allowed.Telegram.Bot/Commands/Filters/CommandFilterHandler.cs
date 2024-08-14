using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Filters;

public abstract class CommandFilterHandler
{
    public virtual Task Initialize(ITelegramBotClient client, Update update)
    {
        return Task.CompletedTask;
    }

    public abstract Task<bool> Apply(params object?[] args);
}