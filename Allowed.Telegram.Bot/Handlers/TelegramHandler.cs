using Allowed.Telegram.Bot.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Handlers;

public sealed class TelegramHandler(IServiceProvider services, ILoggerFactory loggerFactory)
{
    private readonly ILogger<TelegramHandler> _logger = loggerFactory.CreateLogger<TelegramHandler>();

    private IServiceProvider Services { get; } = services;
    private Dictionary<long, TelegramContext> Contexts { get; } = [];

    public void Register(TelegramContext telegramContext)
    {
        Contexts.Add(telegramContext.Client.BotId!.Value, telegramContext);
    }

    public void Unregister(long botId)
    {
        Contexts.Remove(botId);
    }

    public void Unregister(TelegramContext telegramContext)
    {
        Contexts.Remove(telegramContext.Client.BotId!.Value);
    }

    private async Task HandleUpdate(ITelegramBotClient tgClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = Services.CreateAsyncScope();
            await TelegramUpdateHandler.Handle(scope, tgClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex.ToString());
        }
    }

    public async Task HandleWebHookUpdate(long botId, Update update, CancellationToken cancellationToken = default)
    {
        await HandleUpdate(Contexts[botId].Client, update, cancellationToken);
    }

    public async void HandlePollingUpdate(
        ITelegramBotClient tgClient, Update update, CancellationToken cancellationToken = default)
    {
        await HandleUpdate(tgClient, update, cancellationToken);
    }

    public async void PollingErrorHandler(ITelegramBotClient tgClient, Exception exception, CancellationToken _)
    {
        await TelegramUpdateHandler.HandleError(_logger, tgClient, exception);
    }
}