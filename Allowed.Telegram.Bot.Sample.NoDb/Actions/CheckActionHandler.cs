using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Sample.NoDb.Actions;

public class CheckActionHandler(ILogger<CheckActionHandler> logger) : CommandActionHandler
{
    public override Task Execute(ITelegramBotClient client, Update update)
    {
        var sender = SenderHelper.GetSender(update);
        if (sender is { User: not null })
            logger.LogWarning("User {id} is registered!", sender.User.Id);

        return Task.CompletedTask;
    }
}