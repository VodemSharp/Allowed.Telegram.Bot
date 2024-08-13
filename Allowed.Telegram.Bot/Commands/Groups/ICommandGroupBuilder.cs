using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Filters;

namespace Allowed.Telegram.Bot.Commands.Groups;

public interface ICommandGroupBuilder
{
    ICommandGroup Group { get; set; }
    ICommandGroupBuilder AddFilter<TFilter>(params object[] args) where TFilter : CommandFilterHandler;
    ICommandGroupBuilder AddActionBefore<TAction>() where TAction : CommandActionHandler;
    ICommandGroupBuilder AddActionAfter<TAction>() where TAction : CommandActionHandler;

    void ApplyEffects(ICommandBuilder builder);
}