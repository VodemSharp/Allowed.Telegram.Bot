using System.Reflection;

namespace Allowed.Telegram.Bot.Models;

public class TelegramMethod
{
    public Type ControllerType { get; set; }
    public MethodInfo Method { get; set; }
    public string Params { get; set; }
}