using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Data.DbModels.Allowed;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.UserServices;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    public class SampleController : CommandController
    {
        private readonly IUserService<ApplicationTgUser> _userService;
        private readonly IRoleService<ApplicationTgRole> _roleService;

        public SampleController(
            IUserService<ApplicationTgUser> userService,
            IRoleService<ApplicationTgRole> roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [Command("start")]
        public async Task Start(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed: /start");
        }

        //[Command("add_admin_role")]
        //public async Task AddAdminRole(MessageData data)
        //{
        //    if (!_roleService.AnyRole("admin"))
        //        _roleService.AddRole("admin");

        //    if (!_userService.AnyUserRole(data.Message.Chat.Id, "admin"))
        //        _userService.AddUserRole(data.Message.Chat.Id, "admin");

        //    await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You add admin role!");
        //}

        //[Command("remove_admin_role")]
        //public async Task RemoveAdminRole(MessageData data)
        //{
        //    if (_telegramService.AnyUserRole(data.Message.Chat.Id, "admin"))
        //        _telegramService.RemoveUserRole(data.Message.Chat.Id, "admin");

        //    await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You remove admin role!");
        //}

        [DefaultCommand]
        public async Task DefaultCommand(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed unknown command: {data.Message.Text}");
        }
    }
}
