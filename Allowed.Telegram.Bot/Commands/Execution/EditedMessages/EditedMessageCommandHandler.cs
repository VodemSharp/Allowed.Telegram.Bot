using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.EditedMessages;

public class EditedMessageCommandHandler(
    IServiceProvider provider,
    ICommandCollection<EditedMessageCommand> commandCollection)
    : CommandHandler<EditedMessageCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<EditedMessageCommand> commands, CancellationToken token)
    {
        var message = update.EditedMessage!;
        var filteredCommands = commands.Where(x => x.Type == message.Type).ToList();

        List<EditedMessageCommand> result = [];
        if (!string.IsNullOrEmpty(message.Text))
        {
            result = filteredCommands
                .Where(x => x.CheckType == EditedMessageCommandCheckTypes.Strict && x.Text == message.Text).ToList();

            if (result.Count == 0)
            {
                var minCommand = filteredCommands
                    .Where(x => x.CheckType == EditedMessageCommandCheckTypes.Parameterized &&
                                message.Text.StartsWith(x.Text))
                    .MinBy(x => x.Text.Length);

                if (minCommand != null)
                    result.Add(minCommand);
            }
        }

        if (result.Count == 0)
            result = filteredCommands.Where(x => x.Text == string.Empty).ToList();

        return Task.FromResult(result.Count == 0 ? [] : result.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        EditedMessageCommand command, CancellationToken token)
    {
        var message = update.EditedMessage!;
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(Message), _ => message },
                {
                    typeof(EditedMessageCommandArgs), _ => new EditedMessageCommandArgs
                    {
                        Value = command.CheckType == EditedMessageCommandCheckTypes.Parameterized
                            ? message.Text!.Replace(command.Text, string.Empty)
                            : null
                    }
                }
            }));
    }
}