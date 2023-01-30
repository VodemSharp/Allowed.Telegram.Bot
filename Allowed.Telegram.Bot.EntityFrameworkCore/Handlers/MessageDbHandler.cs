using System.Reflection;
using System.Text.Json;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Helpers;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Middlewares;
using Allowed.Telegram.Bot.Enums;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Handlers;

public class MessageDbHandler<TKey, TUser, TRole> : MessageHandler
    where TKey : IEquatable<TKey>
    where TUser : TelegramUser<TKey>
    where TRole : TelegramRole<TKey>
{
    private readonly IRoleService<TKey, TRole> _roleService;
    private readonly IUserService<TKey, TUser> _userService;

    private bool _textAllFilter;

    public MessageDbHandler(ControllersCollection collection, ITelegramBotClient client,
        SimpleTelegramBotClientOptions options,
        IUserService<TKey, TUser> userService, IRoleService<TKey, TRole> roleService, IServiceProvider services)
        : base(collection, client, options, services)
    {
        _userService = userService;
        _roleService = roleService;
    }

    private async Task<string> GetStateValue(long telegramId)
    {
        return await _userService.GetState(telegramId);
    }

    protected override async Task<MethodInfo[]> GetAllowedMethods(long telegramId)
    {
        var userRoles = await _roleService.GetUserRoles(telegramId);
        var state = await GetStateValue(telegramId);

        return ControllerTypes.Where(c =>
            {
                var roles = c.GetRoleAttributes();
                var states = c.GetStateAttributes();

                return (roles.Length == 0 || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ur.Name)))
                       && (states.Length == 0 || states.Any(s => s.GetState() == state));
            })
            .SelectMany(c => c.GetMethods().Where(m =>
            {
                var roles = m.GetRoleAttributes();
                var states = m.GetStateAttributes();

                return (roles.Length == 0 || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ur.Name)))
                       && (states.Length == 0 || states.Any(s => s.GetState() == state));
            })).ToArray();
    }

    protected override async Task<List<MethodInfo>> GetTextCommandMethods(MethodInfo[] methods, Message message)
    {
        return (await base.GetTextCommandMethods(methods, message))
            .Where(m => _textAllFilter || m.GetStateAttributes().Length == 0).ToList();
    }

    protected override async Task<(MethodInfo, string)> GetMethodByText(MethodInfo[] methods, Message message)
    {
        var result = await base.GetMethodByText(methods, message);

        if (result.Item1.GetTextCommandAttributes().Any(a => a.GetText() == null))
        {
            var textMethods = methods.Where(m => m.GetTextCommandAttributes().Any()).ToList();

            if (textMethods.Count != 0)
            {
                MethodInfo[] foundMethods = null;

                var userState = await GetStateValue(message.From!.Id);

                if (!string.IsNullOrEmpty(userState))
                {
                    foundMethods = textMethods.Where(m => m.GetStateAttributes().Any(s => s.GetState() == userState))
                        .ToArray();
                }

                if (foundMethods != null)
                {
                    _textAllFilter = true;
                    var method = await base.GetMethodByText(foundMethods, message);

                    if (method.Item1 != null)
                        return method;
                }
            }
        }

        return result;
    }

    private async Task<object> InvokeMethod(MethodType type, Message message, TKey botId)
    {
        var method = await GetMethod(type, message);

        if (method is { ControllerType: { } } && method.Method != null)
        {
            var methodParams = method.Method.GetParameters();
            var parameters = new List<object>();

            if (methodParams.Any(p => p.ParameterType == typeof(MessageData)))
            {
                parameters.Add(new MessageData
                {
                    Message = message,
                    Client = Client,
                    Options = Options,
                    Params = method.Params
                });
            }

            var controller = (CommandController<TKey>)ActivatorUtilities
                .CreateInstance(Provider, method.ControllerType);

            controller.BotId = botId;

            var serviceFactory = Provider.GetService<IServiceFactory>();

            controller.Initialize(serviceFactory, message.From!.Id);
            await controller.InitializeAsync(serviceFactory, message.From.Id);

            return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
        }

        return null;
    }

    private async Task<object> InvokeCallback(MethodType type, CallbackQuery callback, TKey botId)
    {
        var method = await GetMethod(type, callback);

        if (method is { ControllerType: { } } && method.Method != null)
        {
            var methodParams = method.Method.GetParameters();
            var parameters = new List<object>();

            if (methodParams.Any(p => p.ParameterType == typeof(CallbackQueryData)))
                parameters.Add(new CallbackQueryData
                {
                    Client = Client,
                    CallbackQuery = callback,
                    Options = Options
                });

            var callbackType = typeof(CallbackQueryModel);

            if (methodParams.Any(p => p.ParameterType == callbackType))
                parameters.Add(JsonSerializer.Deserialize(callback.Data, callbackType));

            if (methodParams.Any(p => p.ParameterType.IsSubclassOf(callbackType)))
                parameters.Add(JsonSerializer.Deserialize(
                    callback.Data, methodParams.Single(p => p.ParameterType.IsSubclassOf(callbackType)).ParameterType));

            var controller = (CommandController<TKey>)ActivatorUtilities
                .CreateInstance(Provider, method.ControllerType);

            controller.BotId = botId;

            var serviceFactory = Provider.GetService<IServiceFactory>();

            controller.Initialize(serviceFactory, callback.From.Id);
            await controller.InitializeAsync(serviceFactory, callback.From.Id);

            return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
        }

        return null;
    }

    private async Task<object> InvokeInline(MethodType type, InlineQuery inline, TKey botId)
    {
        var method = await GetMethod(type, inline);

        if (method != null && method.ControllerType != null && method.Method != null)
        {
            var methodParams = method.Method.GetParameters();
            List<object> parameters = new();

            if (methodParams.Any(p => p.ParameterType == typeof(InlineQueryData)))
                parameters.Add(new InlineQueryData
                {
                    InlineQuery = inline,
                    Client = Client,
                    Options = Options
                });

            var controller = (CommandController<TKey>)ActivatorUtilities
                .CreateInstance(Provider, method.ControllerType);

            controller.BotId = botId;

            var serviceFactory = Provider.GetService<IServiceFactory>();

            controller.Initialize(serviceFactory, inline.From.Id);
            await controller.InitializeAsync(serviceFactory, inline.From.Id);

            return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
        }

        return null;
    }

    public async Task<object> OnUpdate(ITelegramBotClient client, Update update, TKey botId, CancellationToken token)
    {
        object result = null;
        var messageMiddleware = Provider.GetService<MessageDbMiddleware<TKey>>();

        try
        {
            if (update.Message != null)
            {
                await _userService.CheckUser(update.Message.From);

                result = await InvokeMethod(GetMethodType(update.Message), update.Message, botId);

                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterMessageProcessed(botId, update.Message.From!.Id);
                    await messageMiddleware.AfterMessageProcessedAsync(botId, update.Message.From.Id);
                }
            }
            else if (update.CallbackQuery != null)
            {
                await _userService.CheckUser(update.CallbackQuery.From);

                if (update.CallbackQuery.Data != null)
                    result = await InvokeCallback(MethodType.Callback, update.CallbackQuery, botId);

                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterCallbackProcessed(botId, update.CallbackQuery.From.Id);
                    await messageMiddleware.AfterCallbackProcessedAsync(botId, update.CallbackQuery.From.Id);
                }
            }
            else if (update.InlineQuery != null)
            {
                await _userService.CheckUser(update.InlineQuery.From);

                result = await InvokeInline(MethodType.Inline, update.InlineQuery, botId);

                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterInlineProcessed(botId, update.InlineQuery.From.Id);
                    await messageMiddleware.AfterInlineProcessedAsync(botId, update.InlineQuery.From.Id);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }

        return result;
    }
}