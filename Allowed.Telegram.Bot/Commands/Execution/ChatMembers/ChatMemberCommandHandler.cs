using Allowed.Telegram.Bot.Commands.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Commands.Execution.ChatMembers;

public class ChatMemberCommandHandler(
    IServiceProvider provider,
    ICommandCollection<ChatMemberCommand> commandCollection)
    : CommandHandler<ChatMemberCommand>(provider, commandCollection)
{
    private readonly IServiceProvider _provider = provider;

    protected override Task<List<Command>> GetCommands(
        ITelegramBotClient client, Update update, List<ChatMemberCommand> commands, CancellationToken token)
    {
        return Task.FromResult(commands.Cast<Command>().ToList());
    }

    protected override Task<List<object?>> GetParameters(ITelegramBotClient client, Update update,
        ChatMemberCommand command, CancellationToken token)
    {
        return Task.FromResult(CommandParamsInjector.GetParameters(_provider, command,
            new Dictionary<Type, Func<Type, object>>
            {
                { typeof(ITelegramBotClient), _ => client },
                { typeof(Update), _ => update },
                { typeof(CancellationToken), _ => token },
                { typeof(ChatMember), _ => update.ChatMember! },
                { typeof(ChatMemberUpdated), _ => update.ChatMember! }
            }));
    }
}