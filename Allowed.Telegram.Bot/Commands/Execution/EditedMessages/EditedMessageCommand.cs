using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Commands.Execution.EditedMessages;

public class EditedMessageCommand : Command
{
    public string Text { get; init; } = null!;
    public MessageType Type { get; init; } = MessageType.Text;
    
    public EditedMessageCommandCheckTypes CheckType { get; init; } = EditedMessageCommandCheckTypes.Strict;
}