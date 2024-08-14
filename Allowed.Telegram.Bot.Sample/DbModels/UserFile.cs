using Allowed.Telegram.Bot.Sample.DbModels.Allowed;

namespace Allowed.Telegram.Bot.Sample.DbModels;

public class UserFile
{
    public int Id { get; set; }

    public long TelegramBotId { get; set; }
    public long TelegramUserId { get; set; }
    public ApplicationTgBotUser TelegramBotUser { get; set; }

    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
}