namespace Allowed.Telegram.Bot.Commands.Core;

public interface ICommandCollection<T>
    where T : Command
{
    public List<T> Items { get; set; }
}