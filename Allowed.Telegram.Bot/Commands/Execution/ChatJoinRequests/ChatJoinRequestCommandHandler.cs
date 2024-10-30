using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.ChatJoinRequests;

public class ChatJoinRequestCommandHandler(
    IServiceProvider provider,
    ICommandCollection<ChatJoinRequestCommand> commandCollection)
    : CommandHandler<ChatJoinRequestCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<ChatJoinRequestCommand> commands, CancellationToken token)
    {
        return Task.FromResult(commands.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        ChatJoinRequestCommand command, CancellationToken token)
    {
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(ChatJoinRequest), _ => update.ChatJoinRequest! }
            }));
    }
}