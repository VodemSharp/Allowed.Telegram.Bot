using Allowed.Telegram.Bot.Commands.Core;

namespace Allowed.Telegram.Bot.Commands.Execution.Messages;

public class MessageCommand : Command
{
    public string Text { get; init; } = null!;
    public MessageCommandTypes Type { get; init; } = MessageCommandTypes.Strict;
}