using System;

namespace Allowed.Telegram.Bot.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EmojiCommandAttribute : Attribute
    {
        private readonly string _smile;

        public EmojiCommandAttribute(string smile)
        {
            _smile = smile;
        }

        public string GetSmile()
        {
            return _smile;
        }
    }
}
