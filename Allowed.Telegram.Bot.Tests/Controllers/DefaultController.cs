using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;

namespace Allowed.Telegram.Bot.Tests.Controllers;

public class DefaultController : CommandController
{
    [DefaultCommand]
    public string DefaultCommand()
    {
        return "DC1";
    }

    [TextCommand]
    public string TextCommand()
    {
        return "DC2";
    }
}