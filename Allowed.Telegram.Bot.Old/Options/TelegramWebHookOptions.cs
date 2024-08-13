namespace Allowed.Telegram.Bot.Options;

public class TelegramWebHookOptions
{
    public string Route { get; set; }
    public bool DeleteOldHooks { get; set; } = true;
}