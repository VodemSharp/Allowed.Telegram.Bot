using Allowed.Telegram.Bot.Commands.Attributes;
using Allowed.Telegram.Bot.Commands.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Attributes;

public class StateAttribute(IUserService userService) : CommandAttributeHandler
{
    private string? _state;

    public override async Task Initialize(ITelegramBotClient client, Update update)
    {
        var userId = SenderHelper.GetSender(update)?.User?.Id;
        if (!client.BotId.HasValue || userId == null) return;

        _state = await userService.GetState(client.BotId.Value, SenderHelper.GetSender(update)!.User!.Id);
    }

    public override Task<bool> Apply(params object?[] args)
    {
        return Task.FromResult(_state == (string)args[0]!);
    }
}