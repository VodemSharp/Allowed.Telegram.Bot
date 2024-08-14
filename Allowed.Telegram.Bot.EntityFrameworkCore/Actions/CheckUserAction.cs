using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Actions;

public class CheckUserAction(IUserService userService) : CommandActionHandler
{
    public override async Task Execute(ITelegramBotClient client, Update update)
    {
        var sender = SenderHelper.GetSender(update);
        if (sender is { User: not null } && client.BotId.HasValue)
        {
            if (!await userService.Any(sender.User.Id))
                await userService.Add(sender.User);
            else
                await userService.Update(sender.User);

            if (!await userService.Any(client.BotId.Value, sender.User.Id))
                await userService.Add(client.BotId.Value, sender.User.Id, sender.User);
            else
                await userService.Update(client.BotId.Value, sender.User.Id, sender.User);
        }
    }
}