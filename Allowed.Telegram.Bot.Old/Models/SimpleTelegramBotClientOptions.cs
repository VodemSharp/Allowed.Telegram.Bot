using Telegram.Bot;

namespace Allowed.Telegram.Bot.Models;

public class SimpleTelegramBotClientOptions : TelegramBotClientOptions
{
    public SimpleTelegramBotClientOptions(string name, string token, string host = null, string baseUrl = null,
        bool useTestEnvironment = false) :
        base(token, baseUrl,
            useTestEnvironment)
    {
        Name = name;
        Host = host;
    }

    public string Name { get; set; }
    public string Host { get; set; }
}