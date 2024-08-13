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

    protected override Task<ExecutableCommand?> GetCommand(
        ITelegramBotClient client, Update update, List<UpdateCommand> commands, CancellationToken token)
    {
        var message = update.Message!;

        if (string.IsNullOrEmpty(message.Text)) return Task.FromResult<ExecutableCommand?>(null);

        var command = commands.SingleOrDefault();

        if (command == null) return Task.FromResult<ExecutableCommand?>(null);

        return Task.FromResult(new ExecutableCommand
        {
            Command = command,
            Parameters = CommandParamsInjector.GetParameters(_provider, command,
                new Dictionary<Type, object>
                {
                    { typeof(ITelegramBotClient), client },
                    { typeof(Update), update },
                    { typeof(CancellationToken), token }
                })
        })!;
    }
}