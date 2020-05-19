using System.Globalization;

namespace Allowed.Telegram.Bot.Helpers
{
    public static class EmojiHelper
    {
        public static bool IsStartEmoji(string text)
        {
            string firstCharOfString = StringInfo.GetNextTextElement(text, 0);
            int result = char.ConvertToUtf32(firstCharOfString, 0);

            return result >= 0x1F600 && result <= 0x1F64F // Emoticons
                 || result >= 0x1F300 && result <= 0x1F5FF // Misc Symbols and Pictographs
                 || result >= 0x1F680 && result <= 0x1F6FF // Transport and Map
                 || result >= 0x2600 && result <= 0x26FF   // Misc symbols
                 || result >= 0x2700 && result <= 0x27BF   // Dingbats
                 || result >= 0xFE00 && result <= 0xFE0F   // Variation Selectors
                 || result >= 0x1F900 && result <= 0x1F9FF // Supplemental Symbols and Pictographs
                 || result >= 0x1F1E6 && result <= 0x1F1FF; // Flags
        }
    }
}
