using System.Text;
using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.Enums;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Sample.DbModels.Allowed;
using Telegram.Bot;

namespace Allowed.Telegram.Bot.Sample.Controllers;

[BotName("Sample")]
public class SampleController : CommandController<int>
{
    private IRoleService<int, ApplicationTgRole> _roleService;

    public override void Initialize(IServiceFactory factory, long telegramId)
    {
        _roleService = factory.CreateRoleService<int, ApplicationTgRole>(BotId);
    }

    [Command("start")]
    public async Task Start(MessageData data)
    {
        var result = "You pressed: /start";
        
        if (!string.IsNullOrEmpty(data.Params))
            result = $"{result}\nParams: {data.Params}";

        await data.Client.SendTextMessageAsync(data.Message.From!.Id, result);
    }

    // You can use only top example for strict also
    [Command("start", Type = ComparisonTypes.Strict)]
    public async Task StrictStart(MessageData data)
    {
        await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You pressed: /start");
    }
    
    [Command("add_admin_role")]
    public async Task AddAdminRole(MessageData data)
    {
        if (!await _roleService.AnyRole("admin")) await _roleService.AddRole(new ApplicationTgRole { Name = "admin" });

        if (await _roleService.AnyUserRole(data.Message.From!.Id, "admin"))
        {
            await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You already have admin role!");
        }
        else
        {
            await _roleService.AddUserRole(data.Message.From!.Id, "admin");
            await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You add admin role!");
        }
    }

    [Command("remove_admin_role")]
    public async Task RemoveAdminRole(MessageData data)
    {
        if (await _roleService.AnyUserRole(data.Message.From!.Id, "admin"))
            await _roleService.RemoveUserRole(data.Message.From!.Id, "admin");

        await data.Client.SendTextMessageAsync(data.Message.From!.Id, "You remove admin role!");
    }

    [DefaultCommand]
    public async Task DefaultCommand(MessageData data)
    {
        await data.Client.SendTextMessageAsync(data.Message.From!.Id,
            $"You pressed unknown command: {data.Message.Text}");
    }
}