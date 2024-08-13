using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Filters;

namespace Allowed.Telegram.Bot.Commands.Core;

public abstract class Command
{
    public Delegate Handler { get; init; } = null!;
    public List<CommandFilter> Filters { get; set; } = [];
    public List<CommandAction> ActionsBefore { get; set; } = [];
    public List<CommandAction> ActionsAfter { get; set; } = [];
}