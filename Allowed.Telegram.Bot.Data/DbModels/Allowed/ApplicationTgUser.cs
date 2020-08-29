using Allowed.Telegram.Bot.Models.Store;
using System.Collections.Generic;

namespace Allowed.Telegram.Bot.Data.DbModels.Allowed
{
    public class ApplicationTgUser : TelegramUser<int>
    {
        public IEnumerable<UserFile> UserFiles { get; set; }
    }
}
