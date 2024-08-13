using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Filters;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Groups;

public interface ICommandGroup
{
    IHost Host { get; set; }
    List<CommandFilter> Filters { get; set; }
    List<CommandAction> ActionsBefore { get; set; }
    List<CommandAction> ActionsAfter { get; set; }
}