using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using System.Threading.Tasks;

namespace Allowed.Telegram.Bot.Tests.Controllers
{
    public class DefaultController : CommandController
    {
        [DefaultCommand]
        public Task DefaultCommand()
        {
            return Task.CompletedTask;
        }
    }
}
