using Allowed.Telegram.Bot.Commands.Filters;
using Allowed.Telegram.Bot.Commands.Helpers;
using Allowed.Telegram.Bot.EntityFrameworkCore.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Filters;

public class RoleFilter(IRoleService roleService) : CommandFilterHandler
{
    private List<string> _roles = [];

    public override async Task Initialize(ITelegramBotClient client, Update update)
    {
        var userId = SenderHelper.GetSender(update)?.User?.Id;
        if (!client.BotId.HasValue || userId == null) return;

        _roles = await roleService.Get(client.BotId.Value, SenderHelper.GetSender(update)!.User!.Id);
    }

    public override Task<bool> Apply(params object?[] args)
    {
        var role = (string)args[0]!;
        return Task.FromResult(_roles.Contains(role));
    }
}