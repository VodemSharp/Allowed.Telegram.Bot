using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Managers.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Managers;

public sealed class TelegramManager(
    IServiceProvider services,
    ILoggerFactory loggerFactory,
    TelegramHandlerList handlerList
) : ITelegramManager
{
    private readonly ILogger<TelegramManager> _logger = loggerFactory.CreateLogger<TelegramManager>();

    private IServiceProvider Services { get; } = services;

    public async Task Start(TelegramHandler telegramHandler)
    {
        if (handlerList.Handlers.Any(c => c.Options.Name == telegramHandler.Options.Name))
        {
            _logger.LogWarning("Telegram bot has already been started!");
            return;
        }

        handlerList.Handlers.Add(telegramHandler);

        await telegramHandler.Client.DeleteWebhookAsync(cancellationToken:
            telegramHandler.CancellationTokenSource?.Token ?? default(CancellationToken));

        // TODO ReceiverOptions
        telegramHandler.Client.StartReceiving(UpdateHandler, PollingErrorHandler,
            new ReceiverOptions(), telegramHandler.CancellationTokenSource?.Token ?? default(CancellationToken));
    }

    private async void UpdateHandler(ITelegramBotClient tgClient, Update update, CancellationToken token)
    {
        using var scope = Services.CreateScope();
        await TelegramUpdateHandler.Handle(scope, tgClient, update, token);
    }

    private async void PollingErrorHandler(ITelegramBotClient tgClient, Exception exception, CancellationToken _)
    {
        await TelegramUpdateHandler.HandleError(_logger, tgClient, exception);
    }

    public async Task Stop(IEnumerable<string> names)
    {
        foreach (var name in names) await Stop(name);
    }

    public Task Stop(string name)
    {
        var handler = handlerList.Handlers.SingleOrDefault(c => c.Options.Name == name);
        if (handler == null)
        {
            _logger.LogWarning("Telegram bot has already been stopped!");
            return Task.CompletedTask;
        }

        handler.CancellationTokenSource?.Cancel();
        handlerList.Handlers.Remove(handler);
        return Task.CompletedTask;
    }

    public async Task Start(IEnumerable<TelegramHandler> handlers)
    {
        foreach (var handler in handlers) await Start(handler);
    }
}