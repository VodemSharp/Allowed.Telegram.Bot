using Telegram.Bot;

namespace Allowed.Telegram.Bot.Clients;

public class SafeTelegramBotClientOptions(
    string token,
    string? baseUrl = null,
    bool useTestEnvironment = false,
    int requests = 30,
    TimeSpan? delay = null,
    List<string>? limitedMethods = null,
    List<string>? exceptLimitedMethods = null)
    : TelegramBotClientOptions(token, baseUrl, useTestEnvironment)
{
    public readonly int Requests = requests;
    public readonly TimeSpan Delay = delay ?? TimeSpan.FromSeconds(1);
    public readonly List<string>? LimitedMethods = limitedMethods;
    public readonly List<string>? ExceptLimitedMethods = exceptLimitedMethods;
}