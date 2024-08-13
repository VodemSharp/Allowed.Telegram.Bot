namespace Allowed.Telegram.Bot.Commands.Core;

public class ExecutableCommand
{
    public Command Command { get; set; } = null!;
    public List<object?> Parameters { get; set; } = null!;
}