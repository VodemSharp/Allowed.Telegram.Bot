using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Commands.Execution.Messages;

public class MessageCommand : Command
{
    public string Text { get; init; } = null!;
    public MessageType Type { get; init; } = MessageType.Text;
    
    public MessageCommandCheckTypes CheckType { get; init; } = MessageCommandCheckTypes.Strict;
}