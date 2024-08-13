namespace Allowed.Telegram.Bot.Commands.Filters;

public class CommandFilter
{
    public Type Handler { get; set; } = null!;
    public object[] Args { get; set; } = null!;
}