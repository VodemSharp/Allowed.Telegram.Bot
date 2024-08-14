using Telegram.Bot;

namespace Allowed.Telegram.Bot.Clients.Options;

public class SimpleTelegramBotClientOptions(
    string token,
    string? host = null,
    string? baseUrl = null,
    bool useTestEnvironment = false)
    : TelegramBotClientOptions(token, baseUrl, useTestEnvironment)
{
    public long Id { get; set; } = long.Parse(token.Split(':')[0]);
    public string? Host { get; set; } = host;
}