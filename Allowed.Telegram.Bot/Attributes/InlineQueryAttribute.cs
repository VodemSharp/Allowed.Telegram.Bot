using System;

namespace Allowed.Telegram.Bot.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InlineQueryAttribute : Attribute
    {
        private readonly string _path;

        public InlineQueryAttribute(string path)
        {
            _path = path;
        }

        public string GetText()
        {
            return _path;
        }
    }
}
