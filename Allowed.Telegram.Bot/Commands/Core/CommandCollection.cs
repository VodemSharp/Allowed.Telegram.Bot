namespace Allowed.Telegram.Bot.Commands.Core;

public class CommandCollection<T> : ICommandCollection<T>
    where T : Command
{
    public List<T> Items { get; set; } = [];
}