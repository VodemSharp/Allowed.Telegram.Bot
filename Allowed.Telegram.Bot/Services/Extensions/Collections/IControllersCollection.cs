using Allowed.Telegram.Bot.Controllers;
using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Services.Extensions.Collections
{
    public interface IControllersCollection
    {
        List<CommandController> Controllers { get; }
    }
}
