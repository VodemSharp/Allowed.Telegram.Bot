namespace Allowed.Telegram.Bot.Commands.Actions;

public class CommandActionGlobalColletion
{
    public List<CommandAction> ActionsBefore { get; set; } = [];
    public List<CommandAction> ActionsAfter { get; set; } = [];
}