using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Options;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;

namespace Allowed.Telegram.Bot.Clients;

public class SafeTelegramBotClient : TelegramBotClient
{
    private readonly List<string> _exceptLimitedRequests;
    private readonly List<string> _limitedRequests;
    private readonly SemaphoreTimeLocker _locker;

    public SafeTelegramBotClient(SafeTelegramBotClientOptions options,
        HttpClient httpClient = null) : base(options,
        httpClient)
    {
        _locker = new SemaphoreTimeLocker(options.Requests, options.Delay);
        _limitedRequests = options.LimitedMethods;
        _exceptLimitedRequests = options.ExceptLimitedMethods;
    }

    public override async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = new())
    {
        if ((_limitedRequests == null || _limitedRequests.Contains(request.MethodName))
            && (_exceptLimitedRequests == null || !_exceptLimitedRequests.Contains(request.MethodName)))
            return await _locker.LockAsync(async () => await base.MakeRequestAsync(request, cancellationToken));

        return await base.MakeRequestAsync(request, cancellationToken);
    }
}