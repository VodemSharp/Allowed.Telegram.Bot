using System.Reflection;
using System.Text.Json;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Enums;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Middlewares;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace Allowed.Telegram.Bot.Handlers;

public class MessageHandler
{
    protected readonly ITelegramBotClient Client;
    protected readonly List<Type> ControllerTypes;
    protected readonly ILogger<MessageHandler> Logger;
    protected readonly SimpleTelegramBotClientOptions Options;

    protected readonly IServiceProvider Provider;

    public MessageHandler(ControllersCollection collection, ITelegramBotClient client,
        SimpleTelegramBotClientOptions options, IServiceProvider services)
    {
        Client = client;
        Options = options;
        ControllerTypes = collection.ControllerTypes
            .Where(c =>
            {
                var attributes = c.GetBotNameAttributes();
                return attributes.Length == 0 || attributes.Any(a => a.GetName() == options.Name);
            }).ToList();

        Provider = services;

        var loggerFactory = Provider.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger<MessageHandler>();
    }

    protected virtual Task<MethodInfo[]> GetAllowedMethods(long telegramId)
    {
        return Task.FromResult(ControllerTypes.SelectMany(c => c.GetMethods()).ToArray());
    }

    protected virtual Task<(MethodInfo, string)> GetMethodByPath(MethodInfo[] methods, Message message)
    {
        var foundMethods = methods.Where(m =>
            m.GetCommandAttributes().Any(a =>
            {
                return a.Type switch
                {
                    ComparisonTypes.Strict => message.Text == $"/{a.GetPath()}",
                    ComparisonTypes.Parameterized => message.Text!.Split(" ").First() == $"/{a.GetPath()}",
                    _ => false
                };
            })).ToList();

        var method = foundMethods.Count > 1
            ? foundMethods.SingleOrDefault(f => f.GetCommandAttributes().Any(a => a.Type == ComparisonTypes.Strict))
            : foundMethods.SingleOrDefault();

        if (method == null)
            method = methods.SingleOrDefault(m => m.GetDefaultCommandAttributes().Any());

        return Task.FromResult((method, string.Join(" ", message.Text!.Split(" ").Skip(1))));
    }

    protected virtual Task<List<MethodInfo>> GetTextCommandMethods(MethodInfo[] methods, Message message)
    {
        return Task.FromResult(methods.Where(m =>
            m.GetTextCommandAttributes().Any(a =>
            {
                return a.Type switch
                {
                    ComparisonTypes.Strict => message.Text == a.GetText(),
                    ComparisonTypes.Parameterized => message.Text!.StartsWith(a.GetText()),
                    _ => false
                };
            })).ToList());
    }

    protected virtual async Task<(MethodInfo, string)> GetMethodByText(MethodInfo[] methods, Message message)
    {
        var foundMethods = await GetTextCommandMethods(methods, message);

        var method = foundMethods.Count > 1
            ? foundMethods.SingleOrDefault(f => f.GetTextCommandAttributes().Any(a => a.Type == ComparisonTypes.Strict))
            : foundMethods.SingleOrDefault();

        if (method == null)
            method = methods.FirstOrDefault(m => m.GetTextCommandAttributes().Any(a => a.GetText() == null));

        string messageParams = null;

        if (method != null)
        {
            var attributes = method.GetTextCommandAttributes().ToList();

            if (attributes.All(a => a.Type != ComparisonTypes.Strict) &&
                attributes.Any(a => a.Type == ComparisonTypes.Parameterized))
            {
                messageParams = message.Text;
                attributes.ForEach(a =>
                {
                    if (messageParams!.StartsWith(a.GetText()))
                        messageParams = messageParams![a.GetText().Length..];
                });
            }
        }

        messageParams = messageParams == string.Empty ? null : messageParams?[1..];

        return (method, messageParams);
    }

    protected virtual Task<(MethodInfo, string)> GetMethodByType(MethodInfo[] methods, Message message)
    {
        return Task.FromResult((methods.SingleOrDefault(m =>
            m.GetTypedCommandAttributes().Any(a => a.GetMessageType() == message.Type)), (string)null));
    }

    protected async Task<TelegramMethod> GetMethod(MethodType type, Message message)
    {
        var allowedMethods = await GetAllowedMethods(message.From!.Id);

        var method = type switch
        {
            MethodType.ByPath => await GetMethodByPath(allowedMethods, message),
            MethodType.Text => await GetMethodByText(allowedMethods, message),
            MethodType.ByType => await GetMethodByType(allowedMethods, message),
            _ => (null, null)
        };

        if (method.Item1 != null)
            return new TelegramMethod
            {
                ControllerType = ControllerTypes.Single(c => c == method.Item1.DeclaringType),
                Method = method.Item1,
                Params = method.Item2
            };

        return null;
    }

    protected async Task<TelegramMethod> GetMethod(MethodType type, CallbackQuery callback)
    {
        MethodInfo method = null;
        var allowedMethods = await GetAllowedMethods(callback.Message!.From!.Id);

        var path = JsonSerializer.Deserialize<CallbackQueryModel>(callback!.Data!)?.Path;

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

    protected async Task<TelegramMethod> GetMethod(MethodType type, PreCheckoutQuery preCheckoutQuery)
    {
        MethodInfo method = null;
        var allowedMethods = await GetAllowedMethods(preCheckoutQuery.From.Id);

        if (type == MethodType.PreCheckout)
            method = allowedMethods.SingleOrDefault(m => m.GetPreCheckoutQueryAttributes().Any());

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
            {
                parameters.Add(new MessageData
                {
                    Message = message,
                    Client = Client,
                    Options = Options,
                    Params = method.Params
                });
            }

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
                    Options = Options
                });

            var callbackType = typeof(CallbackQueryModel);

            if (methodParams.Any(p => p.ParameterType == callbackType))
                parameters.Add(JsonSerializer.Deserialize(callback.Data, callbackType));

            if (methodParams.Any(p => p.ParameterType.IsSubclassOf(callbackType)))
                parameters.Add(JsonSerializer.Deserialize(
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
                    Options = Options
                });

            var controller =
                (CommandController)ActivatorUtilities.CreateInstance(Provider, method.ControllerType);

            controller.Initialize(inline.From.Id);
            await controller.InitializeAsync(inline.From.Id);

            return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
        }

        return null;
    }

    private async Task<object> InvokePreCheckout(MethodType type, PreCheckoutQuery preCheckout)
    {
        var method = await GetMethod(type, preCheckout);

        if (method is { ControllerType: { } } && method.Method != null)
        {
            var methodParams = method.Method.GetParameters();
            var parameters = new List<object>();

            if (methodParams.Any(p => p.ParameterType == typeof(PreCheckoutQueryData)))
                parameters.Add(new PreCheckoutQueryData
                {
                    PreCheckoutQuery = preCheckout,
                    Client = Client,
                    Options = Options
                });

            var controller =
                (CommandController)ActivatorUtilities.CreateInstance(Provider, method.ControllerType);

            controller.Initialize(preCheckout.From.Id);
            await controller.InitializeAsync(preCheckout.From.Id);

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
            else if (update.PreCheckoutQuery != null)
            {
                result = await InvokePreCheckout(MethodType.PreCheckout, update.PreCheckoutQuery);

                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterPreCheckoutProcessed(update.PreCheckoutQuery.From.Id);
                    await messageMiddleware.AfterPreCheckoutProcessedAsync(update.PreCheckoutQuery.From.Id);
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