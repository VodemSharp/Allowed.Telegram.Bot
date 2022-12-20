namespace Allowed.Telegram.Bot.Data.Models;

public class TelegramBotUser<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; }

    public virtual TKey TelegramUserId { get; set; }
    public virtual TKey TelegramBotId { get; set; }

    public virtual bool BotBlocked { get; set; }
    public virtual string State { get; set; }
}