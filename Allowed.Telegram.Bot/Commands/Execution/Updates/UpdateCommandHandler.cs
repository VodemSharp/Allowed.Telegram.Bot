using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.Updates;

public class UpdateCommandHandler(
    IServiceProvider provider,
    ICommandCollection<UpdateCommand> commandCollection)
    : CommandHandler<UpdateCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<UpdateCommand> commands, CancellationToken token)
    {
        return Task.FromResult(commands.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        UpdateCommand command, CancellationToken token)
    {
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, object>
            {
                { typeof(ITelegramBotClient), client },
                { typeof(Update), update },
                { typeof(CancellationToken), token }
            }));
    }
}