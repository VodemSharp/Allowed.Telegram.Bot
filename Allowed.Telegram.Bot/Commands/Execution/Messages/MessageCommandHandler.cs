using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.Messages;

public class MessageCommandHandler(
    IServiceProvider provider,
    ICommandCollection<MessageCommand> commandCollection)
    : CommandHandler<MessageCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<MessageCommand> commands, CancellationToken token)
    {
        var message = update.Message!;
        var filteredCommands = commands.Where(x => x.Type == message.Type).ToList();

        List<MessageCommand> result = [];
        if (!string.IsNullOrEmpty(message.Text))
        {
            result = filteredCommands
                .Where(x => x.CheckType == MessageCommandCheckTypes.Strict && x.Text == message.Text).ToList();

            if (result.Count == 0)
            {
                var minCommand = filteredCommands
                    .Where(x => x.CheckType == MessageCommandCheckTypes.Parameterized &&
                                message.Text.StartsWith(x.Text))
                    .MinBy(x => x.Text.Length);

                if (minCommand != null)
                    result.Add(minCommand);
            }
        }

        if (result.Count == 0) result = filteredCommands.Where(x => x.Text == string.Empty).ToList();
        if (result.Count > 1)
            throw new AmbiguousMessageException(message.Text, message.Type);

        return Task.FromResult(result.Count == 0 ? [] : result.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        MessageCommand command, CancellationToken token)
    {
        var message = update.Message!;
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(Message), _ => message },
                {
                    typeof(MessageCommandArgs), _ => new MessageCommandArgs
                    {
                        Value = command.CheckType == MessageCommandCheckTypes.Parameterized
                            ? message.Text!.Replace(command.Text, string.Empty)
                            : null
                    }
                }
            }));
    }
}