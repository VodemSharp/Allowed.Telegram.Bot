using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Attributes;
using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Filters;

namespace Allowed.Telegram.Bot.Commands.Groups;

public class CommandGroupBuilder : ICommandGroupBuilder
{
    public ICommandGroup Group { get; set; }

    public CommandGroupBuilder(CommandGroup group)
    {
        Group = group;
    }

    public CommandGroupBuilder(ICommandGroupBuilder builder)
    {
        Group = new CommandGroup
        {
            Host = builder.Group.Host,
            Filters = builder.Group.Filters.Select(x => new CommandFilter
            {
                Handler = x.Handler,
                Args = x.Args
            }).ToList(),
            Attributes = builder.Group.Attributes.Select(x => new CommandAttribute
            {
                Handler = x.Handler,
                Args = x.Args
            }).ToList(),
            ActionsBefore = builder.Group.ActionsBefore.Select(x => new CommandAction
            {
                Handler = x.Handler
            }).ToList(),
            ActionsAfter = builder.Group.ActionsAfter.Select(x => new CommandAction
            {
                Handler = x.Handler
            }).ToList()
        };
    }

    public ICommandGroupBuilder AddFilter<TFilter>(params object[] args) where TFilter : CommandFilterHandler
    {
        Group.Filters.Add(new CommandFilter { Handler = typeof(TFilter), Args = args });
        return this;
    }

    public ICommandGroupBuilder AddAttribute<TAttribute>(params object[] args)
        where TAttribute : CommandAttributeHandler
    {
        Group.Attributes.Add(new CommandAttribute { Handler = typeof(TAttribute), Args = args });
        return this;
    }

    public ICommandGroupBuilder AddActionBefore<TAction>() where TAction : CommandActionHandler
    {
        Group.ActionsBefore.Add(new CommandAction { Handler = typeof(TAction) });
        return this;
    }

    public ICommandGroupBuilder AddActionAfter<TAction>() where TAction : CommandActionHandler
    {
        Group.ActionsAfter.Add(new CommandAction { Handler = typeof(TAction) });
        return this;
    }

    public void ApplyEffects(ICommandBuilder builder)
    {
        foreach (var groupFilter in Group.Filters)
            builder.AddFilter(groupFilter);
        
        foreach (var groupAttribute in Group.Attributes)
            builder.AddAttribute(groupAttribute);

        foreach (var action in Group.ActionsBefore)
            builder.AddActionBefore(action);

        foreach (var action in Group.ActionsAfter)
            builder.AddActionBefore(action);
    }
}