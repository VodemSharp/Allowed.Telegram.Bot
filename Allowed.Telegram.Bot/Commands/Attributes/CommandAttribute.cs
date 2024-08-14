namespace Allowed.Telegram.Bot.Commands.Attributes;

public class CommandAttribute
{
    public Type Handler { get; init; } = null!;
    public object?[] Args { get; init; } = null!;
}