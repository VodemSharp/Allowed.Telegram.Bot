using Allowed.Telegram.Bot.Commands.Actions;
using Allowed.Telegram.Bot.Commands.Attributes;
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
using Telegram.Bot.Types.Enums;
using TelegramHandler = Allowed.Telegram.Bot.Handlers.TelegramHandler;

namespace Allowed.Telegram.Bot;

public static class TelegramClientsExtensions
{
    public static IServiceCollection AddTelegramServices(this IServiceCollection services)
    {
        services.AddSingleton<CommandAttributeHandlerCollection>();
        services.AddSingleton<CommandActionGlobalCollection>();
        services.AddSingleton(typeof(ICommandCollection<>), typeof(CommandCollection<>));

        services.AddTransient<MessageCommandHandler>();
        services.AddTransient<InlineQueryCommandHandler>();
        services.AddTransient<ChosenInlineResultCommandHandler>();
        services.AddTransient<CallbackQueryCommandHandler>();
        services.AddTransient<EditedMessageCommandHandler>();
        services.AddTransient<ChannelPostCommandHandler>();
        services.AddTransient<EditedChannelPostCommandHandler>();
        services.AddTransient<ShippingQueryCommandHandler>();
        services.AddTransient<PreCheckoutQueryCommandHandler>();
        services.AddTransient<PollCommandHandler>();
        services.AddTransient<PollAnswerCommandHandler>();
        services.AddTransient<MyChatMemberCommandHandler>();
        services.AddTransient<ChatMemberCommandHandler>();
        services.AddTransient<ChatJoinRequestCommandHandler>();
        services.AddTransient<UpdateCommandHandler>();

        return services;
    }

    public static IServiceCollection AddTelegramHandler(this IServiceCollection services)
    {
        return services.AddSingleton<TelegramHandler>();
    }
}