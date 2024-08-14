using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Actions;

public class CheckBotAction(IBotService botService) : CommandActionHandler
{
    public override async Task Execute(ITelegramBotClient client, Update update)
    {
        if (client.BotId != null && !await botService.Any(client.BotId.Value))
            await botService.Add(client.BotId.Value);
    }
}