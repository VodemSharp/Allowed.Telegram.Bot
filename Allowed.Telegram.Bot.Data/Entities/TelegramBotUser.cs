namespace Allowed.Telegram.Bot.Data.Entities;

public class TelegramBotUser
{
    public virtual long TelegramUserId { get; init; }
    public virtual long TelegramBotId { get; init; }

    public virtual bool Blocked { get; set; }
    public virtual string? State { get; set; }
    
    public virtual bool? AddedToAttachmentMenu { get; set; }
    public virtual DateTime UpdatedAt { get; set; }
}