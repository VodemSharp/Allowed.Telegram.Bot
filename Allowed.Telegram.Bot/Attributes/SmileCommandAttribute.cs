using System;

namespace Allowed.Telegram.Bot.Attributes
{
    public class SmileCommandAttribute : Attribute
    {
        private readonly string _smile;

        public SmileCommandAttribute(string smile)
        {
            _smile = smile;
        }

        public string GetSmile()
        {
            return _smile;
        }
    }
}
