using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Execution.Updates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Managers.Handlers;

public static class TelegramUpdateHandler
{
    public static async Task Handle(IServiceScope scope, ITelegramBotClient client, Update update, CancellationToken token)
    {
        ICommandHandler handler;

        if (update is { Type: UpdateType.Message, Message: { Type: MessageType.Text, Text: not null } })
            handler = scope.ServiceProvider.GetRequiredService<MessageCommandHandler>();
        else
            handler = scope.ServiceProvider.GetRequiredService<UpdateCommandHandler>();

        await handler.Invoke(client, update, token);
    }

    public static Task HandleError(ILogger logger, ITelegramBotClient client, Exception exception)
    {
        logger.LogError("{botId}:\n{exception}", client.BotId, exception.ToString());
        return Task.CompletedTask;
    }
}