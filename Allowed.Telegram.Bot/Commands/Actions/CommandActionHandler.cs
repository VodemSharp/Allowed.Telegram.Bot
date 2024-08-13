using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Actions;

public abstract class CommandActionHandler
{
    public abstract Task Execute(ITelegramBotClient client, Update update);
}