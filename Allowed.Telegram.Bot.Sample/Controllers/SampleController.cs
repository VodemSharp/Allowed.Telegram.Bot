using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.TelegramServices;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    [BotName("Sample")]
    public class SampleController : CommandController
    {
        private readonly ITelegramService _telegramService;

        public SampleController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        [Command("start")]
        public async Task Start(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed: /start");
        }

        [Command("add_admin_role")]
        public async Task AddAdminRole(MessageData data)
        {
            if (!_telegramService.AnyRole("admin"))
                _telegramService.AddRole("admin");

            if (!_telegramService.AnyUserRole(data.Message.Chat.Id, "admin"))
                _telegramService.AddUserRole(data.Message.Chat.Id, "admin");

            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You add admin role!");
        }

        [Command("remove_admin_role")]
        public async Task RemoveAdminRole(MessageData data)
        {
            if (_telegramService.AnyUserRole(data.Message.Chat.Id, "admin"))
                _telegramService.RemoveUserRole(data.Message.Chat.Id, "admin");

            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You remove admin role!");
        }

        [DefaultCommand]
        public async Task DefaultCommand(MessageData data)
        {
            await data.Client.SendTextMessageAsync(data.Message.Chat.Id, $"You pressed unknown command: {data.Message.Text}");
        }
    }
}
