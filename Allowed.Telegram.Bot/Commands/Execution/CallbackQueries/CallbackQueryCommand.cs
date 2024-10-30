using Allowed.Telegram.Bot.Commands.Core;

namespace Allowed.Telegram.Bot.Commands.Execution.CallbackQueries;

public class CallbackQueryCommand : Command
{
    public string Path { get; init; } = null!;
}