using System.Text.Json;
using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.CallbackQueries;

public class CallbackQueryCommandHandler(
    IServiceProvider provider,
    ICommandCollection<CallbackQueryCommand> commandCollection)
    : CommandHandler<CallbackQueryCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<CallbackQueryCommand> commands, CancellationToken token)
    {
        var callbackQuery = update.CallbackQuery!;
        if (string.IsNullOrEmpty(callbackQuery.Data)) return Task.FromResult(new List<Command>());

        var queryData = JsonSerializer.Deserialize<CallbackQueryModel>(callbackQuery.Data);
        if (queryData == null) return Task.FromResult(new List<Command>());

        var result = commands.Where(x => x.Path == queryData.Path).ToList();

        if (result.Count == 0) result = commands.Where(x => x.Path == string.Empty).ToList();
        if (result.Count > 1)
            throw new AmbiguousCallbackQueryException(queryData.Path);

        return Task.FromResult(result.Count == 0 ? [] : result.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        CallbackQueryCommand command, CancellationToken token)
    {
        var data = update.CallbackQuery!.Data!;
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(CallbackQuery), _ => update.CallbackQuery! },
                { typeof(CallbackQueryModel), type => JsonSerializer.Deserialize(data, type)! }
            }));
    }
}