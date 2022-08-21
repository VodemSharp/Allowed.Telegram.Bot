namespace Allowed.Telegram.Bot.EntityFrameworkCore.Extensions.Items;

public class BotsCollection<TKey>
{
    public Dictionary<string, TKey> Values { get; set; } = new();
}