using Telegram.Bot;

namespace Allowed.Telegram.Bot.Clients.Options;

public class SimpleTelegramBotClientOptions(
    string name,
    string token,
    string? host = null,
    string? baseUrl = null,
    bool useTestEnvironment = false)
    : TelegramBotClientOptions(token, baseUrl, useTestEnvironment)
{
    public string Name { get; set; } = name;
    public string? Host { get; set; } = host;
}