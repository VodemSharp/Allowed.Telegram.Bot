using Allowed.Telegram.Bot.Commands.Core;
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
        if (string.IsNullOrEmpty(message.Text)) return Task.FromResult(new List<Command>());

        var messageText = message.Text.Trim();

        // var args = new MessageCommandArgs();
        var result = commands.Where(x => x.Type == MessageCommandTypes.Strict && x.Text == messageText).ToList();

        if (result.Count == 0)
        {
            var minCommand = commands
                .Where(x => x.Type == MessageCommandTypes.Parameterized && messageText.StartsWith(x.Text))
                .MinBy(x => x.Text.Length);

            if (minCommand != null)
                result.Add(minCommand);
        }

        if (result.Count == 0) result = commands.Where(x => x.Text == string.Empty).ToList();
        return Task.FromResult(result.Count == 0 ? [] : result.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        MessageCommand command, CancellationToken token)
    {
        var message = update.Message!.Text!;
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, object>
            {
                { typeof(ITelegramBotClient), client },
                { typeof(Update), update },
                { typeof(CancellationToken), token },
                { typeof(Message), update.Message! },
                {
                    typeof(MessageCommandArgs), new MessageCommandArgs
                    {
                        Value = command.Type == MessageCommandTypes.Parameterized
                            ? message.Replace(command.Text, string.Empty)
                            : null
                    }
                }
            }));
    }
}