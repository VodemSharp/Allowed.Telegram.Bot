namespace Allowed.Telegram.Bot.Clients.Options;

public class SafeTelegramBotClientOptions(
    string name,
    string token,
    string? host = null,
    string? baseUrl = null,
    bool useTestEnvironment = false,
    int requests = 30,
    TimeSpan? delay = null,
    List<string>? limitedMethods = null,
    List<string>? exceptLimitedMethods = null)
    : SimpleTelegramBotClientOptions(name, token, host, baseUrl, useTestEnvironment)
{
    public readonly int Requests = requests;
    public readonly TimeSpan Delay = delay ?? TimeSpan.FromSeconds(1);
    public readonly List<string>? LimitedMethods = limitedMethods;
    public readonly List<string>? ExceptLimitedMethods = exceptLimitedMethods;
}