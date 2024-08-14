using Allowed.Telegram.Bot.Commands.Filters;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Sample.NoDb.Filters;

public class TestFilter : CommandFilterHandler
{
    public override Task Initialize(ITelegramBotClient client, Update update)
    {
        return Task.CompletedTask;
    }

    public override Task<bool> Apply(params object[] args)
    {
        return Task.FromResult((bool)args[0]);
    }
}