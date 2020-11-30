using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;

namespace Allowed.Telegram.Bot.Tests.Controllers
{
    public class DefaultController : CommandController<int>
    {
        [DefaultCommand]
        public string DefaultCommand() => "DC1";

        [TextCommand]
        public string TextCommand() => "DC2";
    }
}
