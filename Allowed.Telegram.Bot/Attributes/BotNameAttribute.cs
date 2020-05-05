using System;

namespace Allowed.Telegram.Bot.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BotNameAttribute : Attribute
    {
        private readonly string _name;

        public BotNameAttribute(string name)
        {
            _name = name;
        }

        public string GetName() => _name;
    }
}
