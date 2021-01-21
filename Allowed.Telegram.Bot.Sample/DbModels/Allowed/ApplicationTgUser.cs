using Allowed.Telegram.Bot.Data.Models;
using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Sample.DbModels.Allowed
{
    public class ApplicationTgUser : TelegramUser<int>
    {
        public IEnumerable<UserFile> UserFiles { get; set; }
    }
}
