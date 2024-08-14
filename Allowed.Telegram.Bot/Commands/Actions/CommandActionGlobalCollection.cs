namespace Allowed.Telegram.Bot.Commands.Actions;

public class CommandActionGlobalCollection
{
    public List<CommandAction> ActionsBefore { get; } = [];
    public List<CommandAction> ActionsAfter { get; } = [];
}