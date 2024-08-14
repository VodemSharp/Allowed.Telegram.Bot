using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Attributes;
using Allowed.Telegram.Bot.Commands.Filters;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Groups;

public interface ICommandGroup
{
    IHost Host { get; }
    List<CommandFilter> Filters { get; }
    List<CommandAttribute> Attributes { get; }
    List<CommandAction> ActionsBefore { get; }
    List<CommandAction> ActionsAfter { get; }
}