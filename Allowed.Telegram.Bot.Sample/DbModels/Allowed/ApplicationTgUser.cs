using Allowed.Telegram.Bot.Data.Models;

namespace Allowed.Telegram.Bot.Sample.DbModels.Allowed;

public class ApplicationTgUser : TelegramUser<int>
{
    public IEnumerable<UserFile> UserFiles { get; set; }
}