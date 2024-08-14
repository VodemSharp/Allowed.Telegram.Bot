namespace Allowed.Telegram.Bot.Data.Entities;

public class TelegramBotUserRole<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual long TelegramBotId { get; init; }
    public virtual long TelegramUserId { get; init; }
    public virtual TKey TelegramRoleId { get; init; } = default!;
}