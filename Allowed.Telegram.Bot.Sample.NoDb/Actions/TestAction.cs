using Allowed.Telegram.Bot.Commands.Actions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Sample.NoDb.Actions;

public class TestAction(ILogger<TestAction> logger) : CommandActionHandler
{
    public override Task Execute(ITelegramBotClient client, Update update)
    {
        logger.LogWarning("This is test action!");
        return Task.CompletedTask;
    }
}