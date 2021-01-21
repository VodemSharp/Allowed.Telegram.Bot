using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Middlewares;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Handlers
{
    public class MessageHandler
    {
        protected readonly List<Type> _controllerTypes;
        protected readonly ITelegramBotClient _client;
        protected readonly BotData _botData;

        protected readonly IServiceProvider _provider;
        protected readonly ILogger<MessageHandler> _logger;

        public MessageHandler(ControllersCollection collection, ITelegramBotClient client,
            BotData botData, IServiceProvider provider)
        {
            _client = client;
            _botData = botData;
            _controllerTypes = collection.ControllerTypes
                .Where(c =>
                {
                    BotNameAttribute[] attributes = c.GetBotNameAttributes();
                    return attributes.Length == 0 || attributes.Any(a => a.GetName() == botData.Name);
                }).ToList();

            _provider = provider;

            ILoggerFactory loggerFactory = provider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<MessageHandler>();
        }

        protected virtual Task<MethodInfo[]> GetAllowedMethods(long telegramId)
        {
            return Task.FromResult(_controllerTypes.SelectMany(c => c.GetMethods()).ToArray());
        }

        protected virtual Task<MethodInfo> GetMethodByPath(MethodInfo[] methods, Message message)
        {
            MethodInfo method = methods.FirstOrDefault(m => m.GetCommandAttributes().Any(a => $"/{a.GetPath()}" == message.Text));

            if (method == null)
                method = methods.FirstOrDefault(m => m.GetDefaultCommandAttributes().Any());

            return Task.FromResult(method);
        }

        protected virtual Task<MethodInfo> GetMethodByText(MethodInfo[] methods, Message message)
        {
            return Task.FromResult(methods.FirstOrDefault(m => m.GetTextCommandAttributes().Any()));
        }

        protected virtual Task<MethodInfo> GetMethodByType(MethodInfo[] methods, Message message)
        {
            return Task.FromResult(methods.FirstOrDefault(m => m.GetTypedCommandAttributes().Any(a => a.GetMessageType() == message.Type)));
        }

        protected async Task<TelegramMethod> GetMethod(MethodType type, Message message)
        {
            MethodInfo method = null;
            MethodInfo[] allowedMethods = await GetAllowedMethods(message.From.Id);

            switch (type)
            {
                case MethodType.ByPath:
                    method = await GetMethodByPath(allowedMethods, message);
                    break;

                case MethodType.Text:
                    method = await GetMethodByText(allowedMethods, message);
                    break;

                case MethodType.ByType:
                    method = await GetMethodByType(allowedMethods, message);
                    break;
            }

            if (method != null)
            {
                return new TelegramMethod
                {
                    ControllerType = _controllerTypes.First(c => c == method.DeclaringType),
                    Method = method
                };
            }

            return null;
        }

        protected async Task<TelegramMethod> GetMethod(MethodType type, CallbackQuery callback)
        {
            MethodInfo method = null;
            MethodInfo[] allowedMethods = await GetAllowedMethods(callback.Message.From.Id);

            string path = JsonConvert.DeserializeObject<CallbackQueryModel>(callback.Data).Path;

            if (type == MethodType.Callback)
                method = allowedMethods.FirstOrDefault(m => m.GetCallbackQueryAttributes().Any(a => a.GetPath() == path));

            if (method == null)
                method = allowedMethods.FirstOrDefault(m => m.GetCallbackDefaultQueryAttributes().Any());

            if (method != null)
            {
                return new TelegramMethod
                {
                    ControllerType = _controllerTypes.First(c => c == method.DeclaringType),
                    Method = method
                };
            }

            return null;
        }

        protected async Task<TelegramMethod> GetMethod(MethodType type, InlineQuery inline)
        {
            MethodInfo method = null;
            MethodInfo[] allowedMethods = await GetAllowedMethods(inline.From.Id);

            if (type == MethodType.Inline)
            {
                method = allowedMethods.FirstOrDefault(m => m.GetInlineQueryAttributes().Any());
            }

            if (method != null)
            {
                return new TelegramMethod
                {
                    ControllerType = _controllerTypes.First(c => c == method.DeclaringType),
                    Method = method
                };
            }

            return null;
        }

        private async Task<object> InvokeMethod(MethodType type, Message message)
        {
            TelegramMethod method = await GetMethod(type, message);

            if (method != null && method.ControllerType != null && method.Method != null)
            {
                ParameterInfo[] methodParams = method.Method.GetParameters();
                List<object> parameters = new List<object> { };

                if (methodParams.Any(p => p.ParameterType == typeof(MessageData)))
                {
                    parameters.Add(new MessageData
                    {
                        Message = message,
                        Client = _client,
                        BotData = _botData
                    });
                }

                CommandController controller =
                    (CommandController)ActivatorUtilities.CreateInstance(_provider, method.ControllerType);

                controller.Initialize(message.From.Id);
                await controller.InitializeAsync(message.From.Id);

                return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
            }

            return null;
        }

        private async Task<object> InvokeCallback(MethodType type, CallbackQuery callback)
        {
            TelegramMethod method = await GetMethod(type, callback);

            if (method != null && method.ControllerType != null && method.Method != null)
            {
                ParameterInfo[] methodParams = method.Method.GetParameters();
                List<object> parameters = new List<object> { };

                if (methodParams.Any(p => p.ParameterType == typeof(CallbackQueryData)))
                {
                    parameters.Add(new CallbackQueryData
                    {
                        Client = _client,
                        CallbackQuery = callback,
                        BotData = _botData
                    });
                }

                Type callbackType = typeof(CallbackQueryModel);

                if (methodParams.Any(p => p.ParameterType == callbackType))
                    parameters.Add(JsonConvert.DeserializeObject(callback.Data, callbackType));

                if (methodParams.Any(p => p.ParameterType.IsSubclassOf(callbackType)))
                    parameters.Add(JsonConvert.DeserializeObject(
                        callback.Data, methodParams.First(p => p.ParameterType.IsSubclassOf(callbackType)).ParameterType));

                CommandController controller =
                    (CommandController)ActivatorUtilities.CreateInstance(_provider, method.ControllerType);

                controller.Initialize(callback.From.Id);
                await controller.InitializeAsync(callback.From.Id);

                return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
            }

            return null;
        }

        private async Task<object> InvokeInline(MethodType type, InlineQuery inline)
        {
            TelegramMethod method = await GetMethod(type, inline);

            if (method != null && method.ControllerType != null && method.Method != null)
            {
                ParameterInfo[] methodParams = method.Method.GetParameters();
                List<object> parameters = new List<object> { };

                if (methodParams.Any(p => p.ParameterType == typeof(InlineQueryData)))
                {
                    parameters.Add(new InlineQueryData
                    {
                        InlineQuery = inline,
                        Client = _client,
                        BotData = _botData
                    });
                }

                CommandController controller =
                    (CommandController)ActivatorUtilities.CreateInstance(_provider, method.ControllerType);

                controller.Initialize(inline.From.Id);
                await controller.InitializeAsync(inline.From.Id);

                return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
            }

            return null;
        }

        protected virtual MethodType GetMethodType(Message message)
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    if (message.Text.StartsWith("/"))
                        return MethodType.ByPath;
                    else
                        return MethodType.Text;
                default:
                    return MethodType.ByType;
            }
        }

        public async Task<object> OnMessage(MessageEventArgs e)
        {
            Message message = e.Message;
            object result = null;

            try
            {
                result = await InvokeMethod(GetMethodType(message), message);

                MessageMiddleware messageMiddleware = _provider.GetService<MessageMiddleware>();
                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterMessageProcessed(e.Message.From.Id);
                    await messageMiddleware.AfterMessageProcessedAsync(e.Message.From.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }

        public async Task<object> OnCallbackQuery(CallbackQueryEventArgs e)
        {
            CallbackQuery callback = e.CallbackQuery;
            object result = null;

            try
            {
                if (e.CallbackQuery.Data != null)
                    result = await InvokeCallback(MethodType.Callback, callback);

                MessageMiddleware messageMiddleware = _provider.GetService<MessageMiddleware>();
                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterCallbackProcessed(e.CallbackQuery.From.Id);
                    await messageMiddleware.AfterCallbackProcessedAsync(e.CallbackQuery.From.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }

        public async Task<object> OnInlineQuery(InlineQueryEventArgs e)
        {
            InlineQuery inline = e.InlineQuery;
            object result = null;

            try
            {
                result = await InvokeInline(MethodType.Inline, inline);

                MessageMiddleware messageMiddleware = _provider.GetService<MessageMiddleware>();
                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterInlineProcessed(e.InlineQuery.From.Id);
                    await messageMiddleware.AfterInlineProcessedAsync(e.InlineQuery.From.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }
    }
}
