using System.Reflection;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Middlewares;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Handlers;

public class MessageHandler
{
    protected readonly BotData BotData;
    protected readonly ITelegramBotClient Client;
    protected readonly List<Type> ControllerTypes;
    protected readonly ILogger<MessageHandler> Logger;

    protected readonly IServiceProvider Provider;

    public MessageHandler(ControllersCollection collection, ITelegramBotClient client,
        BotData botData, IServiceProvider provider)
    {
        Client = client;
        BotData = botData;
        ControllerTypes = collection.ControllerTypes
            .Where(c =>
            {
                var attributes = c.GetBotNameAttributes();
                return attributes.Length == 0 || attributes.Any(a => a.GetName() == botData.Name);
            }).ToList();

        Provider = provider;

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger<MessageHandler>();
    }

    protected virtual Task<MethodInfo[]> GetAllowedMethods(long telegramId)
    {
        return Task.FromResult(ControllerTypes.SelectMany(c => c.GetMethods()).ToArray());
    }

    protected virtual Task<MethodInfo> GetMethodByPath(MethodInfo[] methods, Message message)
    {
        var method = methods.SingleOrDefault(m => m.GetCommandAttributes().Any(a => $"/{a.GetPath()}" == message.Text));

        if (method == null)
            method = methods.SingleOrDefault(m => m.GetDefaultCommandAttributes().Any());

        return Task.FromResult(method);
    }

    protected virtual Task<MethodInfo> GetMethodByText(MethodInfo[] methods, Message message)
    {
        return Task.FromResult(methods.SingleOrDefault(m => m.GetTextCommandAttributes().Any()));
    }

    protected virtual Task<MethodInfo> GetMethodByType(MethodInfo[] methods, Message message)
    {
        return Task.FromResult(methods.SingleOrDefault(m =>
            m.GetTypedCommandAttributes().Any(a => a.GetMessageType() == message.Type)));
    }

    protected async Task<TelegramMethod> GetMethod(MethodType type, Message message)
    {
        var allowedMethods = await GetAllowedMethods(message.From!.Id);

        var method = type switch
        {
            MethodType.ByPath => await GetMethodByPath(allowedMethods, message),
            MethodType.Text => await GetMethodByText(allowedMethods, message),
            MethodType.ByType => await GetMethodByType(allowedMethods, message),
            _ => null
        };

        if (method != null)
            return new TelegramMethod
            {
                ControllerType = ControllerTypes.Single(c => c == method.DeclaringType),
                Method = method
            };

        return null;
    }

    protected async Task<TelegramMethod> GetMethod(MethodType type, CallbackQuery callback)
    {
        MethodInfo method = null;
        var allowedMethods = await GetAllowedMethods(callback.Message!.From!.Id);

        var path = JsonConvert.DeserializeObject<CallbackQueryModel>(callback.Data).Path;

        if (type == MethodType.Callback)
            method = allowedMethods.SingleOrDefault(m => m.GetCallbackQueryAttributes().Any(a => a.GetPath() == path));

        if (method == null)
            method = allowedMethods.SingleOrDefault(m => m.GetCallbackDefaultQueryAttributes().Any());

        if (method != null)
            return new TelegramMethod
            {
                ControllerType = ControllerTypes.Single(c => c == method.DeclaringType),
                Method = method
            };

        return null;
    }

    protected async Task<TelegramMethod> GetMethod(MethodType type, InlineQuery inline)
    {
        MethodInfo method = null;
        var allowedMethods = await GetAllowedMethods(inline.From.Id);

        if (type == MethodType.Inline) method = allowedMethods.SingleOrDefault(m => m.GetInlineQueryAttributes().Any());

        if (method != null)
            return new TelegramMethod
            {
                ControllerType = ControllerTypes.Single(c => c == method.DeclaringType),
                Method = method
            };

        return null;
    }

    private async Task<object> InvokeMethod(MethodType type, Message message)
    {
        var method = await GetMethod(type, message);

        if (method is { ControllerType: { } } && method.Method != null)
        {
            var methodParams = method.Method.GetParameters();
            var parameters = new List<object>();

            if (methodParams.Any(p => p.ParameterType == typeof(MessageData)))
                parameters.Add(new MessageData
                {
                    Message = message,
                    Client = Client,
                    BotData = BotData
                });

            var controller =
                (CommandController)ActivatorUtilities.CreateInstance(Provider, method.ControllerType);

            controller.Initialize(message.From!.Id);
            await controller.InitializeAsync(message.From.Id);

            return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
        }

        return null;
    }

    private async Task<object> InvokeCallback(MethodType type, CallbackQuery callback)
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
                    BotData = BotData
                });

            var callbackType = typeof(CallbackQueryModel);

            if (methodParams.Any(p => p.ParameterType == callbackType))
                parameters.Add(JsonConvert.DeserializeObject(callback.Data, callbackType));

            if (methodParams.Any(p => p.ParameterType.IsSubclassOf(callbackType)))
                parameters.Add(JsonConvert.DeserializeObject(
                    callback.Data, methodParams.Single(p => p.ParameterType.IsSubclassOf(callbackType)).ParameterType));

            var controller =
                (CommandController)ActivatorUtilities.CreateInstance(Provider, method.ControllerType);

            controller.Initialize(callback.From.Id);
            await controller.InitializeAsync(callback.From.Id);

            return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
        }

        return null;
    }

    private async Task<object> InvokeInline(MethodType type, InlineQuery inline)
    {
        var method = await GetMethod(type, inline);

        if (method is { ControllerType: { } } && method.Method != null)
        {
            var methodParams = method.Method.GetParameters();
            var parameters = new List<object>();

            if (methodParams.Any(p => p.ParameterType == typeof(InlineQueryData)))
                parameters.Add(new InlineQueryData
                {
                    InlineQuery = inline,
                    Client = Client,
                    BotData = BotData
                });

            var controller =
                (CommandController)ActivatorUtilities.CreateInstance(Provider, method.ControllerType);

            controller.Initialize(inline.From.Id);
            await controller.InitializeAsync(inline.From.Id);

            return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
        }

        return null;
    }

    protected virtual MethodType GetMethodType(Message message)
    {
        return message.Type switch
        {
            MessageType.Text => message.Text!.StartsWith("/") ? MethodType.ByPath : MethodType.Text,
            _ => MethodType.ByType
        };
    }

    public async Task<object> OnUpdate(ITelegramBotClient client, Update update, CancellationToken? cancellationToken)
    {
        object result = null;

        try
        {
            var messageMiddleware = Provider.GetService<MessageMiddleware>();
            if (update.Message != null)
            {
                result = await InvokeMethod(GetMethodType(update.Message), update.Message);

                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterMessageProcessed(update.Message.From!.Id);
                    await messageMiddleware.AfterMessageProcessedAsync(update.Message.From.Id);
                }
            }
            else if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data != null)
                    result = await InvokeCallback(MethodType.Callback, update.CallbackQuery);

                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterCallbackProcessed(update.CallbackQuery.From.Id);
                    await messageMiddleware.AfterCallbackProcessedAsync(update.CallbackQuery.From.Id);
                }
            }
            else if (update.InlineQuery != null)
            {
                result = await InvokeInline(MethodType.Inline, update.InlineQuery);

                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterInlineProcessed(update.InlineQuery.From.Id);
                    await messageMiddleware.AfterInlineProcessedAsync(update.InlineQuery.From.Id);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("{exception}", ex.ToString());
        }

        return result;
    }
}