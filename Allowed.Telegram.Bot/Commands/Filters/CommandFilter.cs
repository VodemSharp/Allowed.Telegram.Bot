namespace Allowed.Telegram.Bot.Commands.Filters;

public class CommandFilter
{
    public Type Handler { get; init; } = null!;
    public object?[] Args { get; init; } = null!;
}