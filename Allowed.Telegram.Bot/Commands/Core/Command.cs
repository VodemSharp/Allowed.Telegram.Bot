using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Attributes;
using Allowed.Telegram.Bot.Commands.Filters;

namespace Allowed.Telegram.Bot.Commands.Core;

public abstract class Command
{
    public Delegate Handler { get; init; } = null!;
    public List<CommandFilter> Filters { get; } = [];
    public List<CommandAttribute> Attributes { get; } = [];
    public List<CommandAction> ActionsBefore { get; } = [];
    public List<CommandAction> ActionsAfter { get; } = [];
}