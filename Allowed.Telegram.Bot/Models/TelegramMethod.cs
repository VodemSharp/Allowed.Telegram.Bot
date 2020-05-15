using Allowed.Telegram.Bot.Controllers;
using System.Reflection;

namespace Allowed.Telegram.Bot.Models
{
    public class TelegramMethod
    {
        public CommandController Controller { get; set; }
        public MethodInfo Method { get; set; }
    }
}
