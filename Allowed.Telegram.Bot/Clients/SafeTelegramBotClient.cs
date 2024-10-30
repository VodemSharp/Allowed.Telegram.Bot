using Allowed.Telegram.Bot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;

namespace Allowed.Telegram.Bot.Clients;

public class SafeTelegramBotClient(
    SafeTelegramBotClientOptions options,
    HttpClient? httpClient = null
) : TelegramBotClient(options, httpClient)
{
    private readonly List<string>? _exceptLimitedRequests = options.ExceptLimitedMethods;
    private readonly List<string>? _limitedRequests = options.LimitedMethods;
    private readonly SemaphoreTimeLocker _locker = new(options.Requests, options.Delay);

    public override async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = new())
    {
        if ((_limitedRequests == null || _limitedRequests.Contains(request.MethodName))
            && (_exceptLimitedRequests == null || !_exceptLimitedRequests.Contains(request.MethodName)))
            return await _locker.LockAsync(async () => await base.MakeRequestAsync(request, cancellationToken));

        return await base.MakeRequestAsync(request, cancellationToken);
    }
}