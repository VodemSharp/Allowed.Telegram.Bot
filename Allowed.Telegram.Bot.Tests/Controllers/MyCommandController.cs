using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;

namespace Allowed.Telegram.Bot.Tests.Controllers
{
    public class MyCommandController : CommandController
    {
        [Command("start")]
        public string Start() => "MC1";
    }
}
