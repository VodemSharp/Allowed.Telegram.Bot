using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Commands.Helpers;

public class Sender(Chat chat, User? user)
{
    public Chat Chat { get; } = chat;
    public User? User { get; } = user;
}

public static class SenderHelper
{
    public static Sender? GetSender(Update update)
    {
        if (update is { Type: UpdateType.Message, Message: not null })
            return new Sender(update.Message.Chat, update.Message.From);

        return null;
    }
}