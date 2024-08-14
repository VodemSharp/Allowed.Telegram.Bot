namespace Allowed.Telegram.Bot.Data.Entities;

public class TelegramRole<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; init; } = default!;
    public virtual string Name { get; init; } = null!;
}