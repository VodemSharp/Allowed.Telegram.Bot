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

    protected override Task<ExecutableCommand?> GetCommand(
        ITelegramBotClient client, Update update, List<MessageCommand> commands, CancellationToken token)
    {
        var message = update.Message!;
        if (string.IsNullOrEmpty(message.Text)) return Task.FromResult<ExecutableCommand?>(null);

        var messageText = message.Text.Trim();

        var args = new MessageCommandArgs();
        var command = commands.SingleOrDefault(x => x.Type == MessageCommandTypes.Strict && x.Text == messageText);

        if (command == null)
        {
            command = commands
                .Where(x => x.Type == MessageCommandTypes.Parameterized && messageText.StartsWith(x.Text))
                .MinBy(x => x.Text.Length);

            if (command != null)
                args.Value = messageText.Replace(command.Text, string.Empty);
        }

        command ??= commands.SingleOrDefault(x => x.Text == string.Empty);
        if (command == null) return Task.FromResult<ExecutableCommand?>(null);

        return Task.FromResult(new ExecutableCommand
        {
            Command = command,
            Parameters = CommandParamsInjector.GetParameters(_provider, command,
                new Dictionary<Type, object>
                {
                    { typeof(ITelegramBotClient), client },
                    { typeof(Update), update },
                    { typeof(CancellationToken), token },
                    { typeof(Message), message },
                    { typeof(MessageCommandArgs), args }
                })
        })!;
    }
}