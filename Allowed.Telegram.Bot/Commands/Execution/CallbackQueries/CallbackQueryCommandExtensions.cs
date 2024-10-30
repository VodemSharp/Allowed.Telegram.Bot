using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Groups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Allowed.Telegram.Bot.Commands.Execution.CallbackQueries;

public static class CallbackQueryCommandExtensions
{
    public static ICommandBuilder MapCallbackQueryCommand(this IHost host, string path, Delegate handler)
    {
        var collection = host.Services.GetRequiredService<ICommandCollection<CallbackQueryCommand>>();
        var command = new CallbackQueryCommand
        {
            Path = path,
            Handler = handler
        };

        collection.Items.Add(command);

        return new CommandBuilder<CallbackQueryCommand>(command);
    }

    public static ICommandBuilder MapCallbackQueryCommand(
        this ICommandGroupBuilder builder, string path, Delegate handler)
    {
        var command = builder.Group.Host.MapCallbackQueryCommand(path, handler);
        builder.ApplyEffects(command);

        return command;
    }

    public static ICommandBuilder MapCallbackQueryCommand(this IHost host, Delegate handler)
    {
        return host.MapCallbackQueryCommand(string.Empty, handler);
    }

    public static ICommandBuilder MapCallbackQueryCommand(this ICommandGroupBuilder builder, Delegate handler)
    {
        var command = builder.Group.Host.MapCallbackQueryCommand(string.Empty, handler);
        foreach (var groupFilter in builder.Group.Filters)
            command.AddFilter(groupFilter);

        return command;
    }
}