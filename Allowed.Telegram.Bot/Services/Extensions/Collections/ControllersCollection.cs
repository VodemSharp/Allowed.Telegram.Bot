using Allowed.Telegram.Bot.Controllers;
using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Services.Extensions.Collections
{
    public class ControllersCollection : IControllersCollection
    {
        public List<CommandController> Controllers { get; set; }
    }
}
