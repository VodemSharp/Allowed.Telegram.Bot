using System;

namespace Allowed.Telegram.Bot.Attributes
{
    public class CommandAttribute : Attribute
    {
        private readonly string _path;

        public CommandAttribute(string path)
        {
            _path = path;
        }

        public string GetPath()
        {
            return _path;
        }
    }
}
