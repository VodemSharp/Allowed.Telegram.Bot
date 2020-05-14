using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Services.TelegramServices;

namespace Allowed.Telegram.Bot.Sample.Controllers
{
    public class RolesController : CommandController
    {
        private readonly ITelegramService _telegramService;
        
        public RolesController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }
    }
}
