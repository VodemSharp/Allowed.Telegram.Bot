namespace Allowed.Telegram.Bot.Commands.Actions;

public class CommandAction
{
    public Type Handler { get; init; } = null!;
}