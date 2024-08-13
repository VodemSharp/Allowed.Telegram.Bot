using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Filters;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Groups;

public class CommandGroup : ICommandGroup
{
    public IHost Host { get; set; } = null!;
    public List<CommandFilter> Filters { get; set; } = [];
    public List<CommandAction> ActionsBefore { get; set; } = [];
    public List<CommandAction> ActionsAfter { get; set; } = [];
}