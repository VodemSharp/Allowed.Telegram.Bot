namespace Allowed.Telegram.Bot.Data.Entities;

public class TelegramUser
{
    public virtual long Id { get; init; }

    public bool IsBot { get; set; }
    public virtual string FirstName { get; set; } = null!;
    public virtual string? LastName { get; set; }
    public virtual string? Username { get; set; }
    public virtual string? LanguageCode { get; set; }
    public virtual bool? IsPremium { get; set; }
    
    public virtual DateTime UpdatedAt { get; set; }
}