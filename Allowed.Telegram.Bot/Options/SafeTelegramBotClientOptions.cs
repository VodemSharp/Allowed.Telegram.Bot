using Allowed.Telegram.Bot.Models;

namespace Allowed.Telegram.Bot.Options;

public class SafeTelegramBotClientOptions : SimpleTelegramBotClientOptions
{
    public readonly TimeSpan Delay;
    public readonly List<string> ExceptLimitedMethods;
    public readonly List<string> LimitedMethods;
    public readonly int Requests;

    public SafeTelegramBotClientOptions(string name, string token, string host = null, int requests = 30,
        TimeSpan? delay = null,
        List<string> limitedMethods = null,
        List<string> exceptLimitedMethods = null,
        string baseUrl = null,
        bool useTestEnvironment = false) : base(
        name, token, host, baseUrl, useTestEnvironment)
    {
        Requests = requests;
        Delay = delay ?? TimeSpan.FromSeconds(1);
        LimitedMethods = limitedMethods;
        ExceptLimitedMethods = exceptLimitedMethods;
    }
}