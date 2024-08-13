namespace Allowed.Telegram.Bot.Sample.NoDb.Models;

public class TelegramBotDto
{
    public string Name { get; set; }
    public bool Started { get; set; }

    public TelegramBotDto(string name, bool started)
    {
        Name = name;
        Started = started;
    }
}