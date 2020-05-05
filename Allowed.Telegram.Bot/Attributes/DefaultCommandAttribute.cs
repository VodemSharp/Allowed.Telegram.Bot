using System;

namespace Allowed.Telegram.Bot.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DefaultCommandAttribute : Attribute
    {
    }
}
