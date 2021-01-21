using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Data.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Models;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [Role("admin")]
    [BotName("Sample")]
    public class RolesController : CommandController<int>
    {
        [Command("admin_role_controller")]
        public async Task RoleControllerMethod(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You have permission for this command in controller!");
        }
    }
}
