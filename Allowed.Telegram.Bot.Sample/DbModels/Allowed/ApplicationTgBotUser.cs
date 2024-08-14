using Allowed.Telegram.Bot.Data.Entities;

namespace Allowed.Telegram.Bot.Sample.DbModels.Allowed;

public class ApplicationTgBotUser : TelegramBotUser
{
    public IEnumerable<UserFile> UserFiles { get; set; }
}