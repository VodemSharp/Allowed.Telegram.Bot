using Allowed.Telegram.Bot.Commands.Core;
using Allowed.Telegram.Bot.Commands.Execution.CallbackQueries;
using Allowed.Telegram.Bot.Commands.Execution.ChannelPosts;
using Allowed.Telegram.Bot.Commands.Execution.ChatJoinRequests;
using Allowed.Telegram.Bot.Commands.Execution.ChatMembers;
using Allowed.Telegram.Bot.Commands.Execution.ChosenInlineResults;
using Allowed.Telegram.Bot.Commands.Execution.EditedChannelPosts;
using Allowed.Telegram.Bot.Commands.Execution.EditedMessages;
using Allowed.Telegram.Bot.Commands.Execution.InlineQueries;
using Allowed.Telegram.Bot.Commands.Execution.Messages;
using Allowed.Telegram.Bot.Commands.Execution.MyChatMembers;
using Allowed.Telegram.Bot.Commands.Execution.PollAnswers;
using Allowed.Telegram.Bot.Commands.Execution.Polls;
using Allowed.Telegram.Bot.Commands.Execution.PreCheckoutQueries;
using Allowed.Telegram.Bot.Commands.Execution.ShippingQueries;
using Allowed.Telegram.Bot.Commands.Execution.Updates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Handlers;

public static class TelegramUpdateHandler
{
    public static async Task Handle(
        IServiceScope scope, ITelegramBotClient client, Update update, CancellationToken token)
    {
        var handlerType = update switch
        {
            { Type: UpdateType.Message } => typeof(MessageCommandHandler),
            { Type: UpdateType.InlineQuery } => typeof(InlineQueryCommandHandler),
            { Type: UpdateType.ChosenInlineResult } => typeof(ChosenInlineResultCommandHandler),
            { Type: UpdateType.CallbackQuery } => typeof(CallbackQueryCommandHandler),
            { Type: UpdateType.EditedMessage } => typeof(EditedMessageCommandHandler),
            { Type: UpdateType.ChannelPost } => typeof(ChannelPostCommandHandler),
            { Type: UpdateType.EditedChannelPost } => typeof(EditedChannelPostCommandHandler),
            { Type: UpdateType.ShippingQuery } => typeof(ShippingQueryCommandHandler),
            { Type: UpdateType.PreCheckoutQuery } => typeof(PreCheckoutQueryCommandHandler),
            { Type: UpdateType.Poll } => typeof(PollCommandHandler),
            { Type: UpdateType.PollAnswer } => typeof(PollAnswerCommandHandler),
            { Type: UpdateType.MyChatMember } => typeof(MyChatMemberCommandHandler),
            { Type: UpdateType.ChatMember } => typeof(ChatMemberCommandHandler),
            { Type: UpdateType.ChatJoinRequest } => typeof(ChatJoinRequestCommandHandler),
            _ => typeof(UpdateCommandHandler)
        };

        var handler = (ICommandHandler)scope.ServiceProvider.GetRequiredService(handlerType);
        await handler.Invoke(client, update, token);
    }

    public static Task HandleError(ILogger logger, ITelegramBotClient client, Exception exception)
    {
        logger.LogError("{botId}:\n{exception}", client.BotId, exception.ToString());
        return Task.CompletedTask;
    }
}