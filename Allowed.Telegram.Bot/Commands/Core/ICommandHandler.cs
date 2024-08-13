using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Core;

public interface ICommandHandler
{
    Task Invoke(ITelegramBotClient client, Update update, CancellationToken token);
}