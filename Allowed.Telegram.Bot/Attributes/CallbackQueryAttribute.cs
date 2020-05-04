using System;

namespace Allowed.Telegram.Bot.Attributes
{
    public class CallbackQueryAttribute : Attribute
    {
        private readonly string _path;

        public CallbackQueryAttribute(string path)
        {
            _path = path;
        }

        public string GetPath()
        {
            return _path;
        }
    }
}
