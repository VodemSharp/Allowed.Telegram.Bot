using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.DependencyInjection;
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

namespace Allowed.Telegram.Bot.Handlers.MessageHandlers
{
    public class MessageHandler
    {
        protected readonly List<Type> _controllerTypes;
        protected readonly ITelegramBotClient _client;
        protected readonly BotData _botData;

        protected readonly IServiceProvider _provider;

        public MessageHandler(ControllersCollection collection, ITelegramBotClient client,
            BotData botData, IServiceProvider provider)
        {
            _client = client;
            _botData = botData;
            _controllerTypes = collection.ControllerTypes
                .Where(c =>
                {
                    BotNameAttribute[] attributes = GetBotNameAttributes(c);
                    return attributes.Length == 0 || attributes.Any(a => a.GetName() == botData.Name);
                }).ToList();

            _provider = provider;
        }

        protected BotNameAttribute[] GetBotNameAttributes(Type controllerType)
        {
            return (BotNameAttribute[])controllerType.GetCustomAttributes(typeof(BotNameAttribute), false);
        }

        protected enum MethodType
        {
            ByPath, BySmile, ByType, Text, Callback
        }

        protected virtual Task<MethodInfo[]> GetAllowedMethods(long chatId)
        {
            return Task.FromResult(_controllerTypes.SelectMany(c => c.GetMethods()).ToArray());
        }

        protected virtual Task<string> GetStateValue(long chatId = 0) => Task.FromResult(string.Empty);

        protected async Task<TelegramMethod> GetMethod(MethodType type, Message message)
        {
            MethodInfo method = null;
            MethodInfo[] allowedMethods = await GetAllowedMethods(message.Chat.Id);

            if (type == MethodType.ByPath)
            {
                method = allowedMethods
                            .FirstOrDefault(m => ((CommandAttribute[])m.GetCustomAttributes(typeof(CommandAttribute), false))
                            .Any(a => $"/{a.GetPath()}" == message.Text));

                if (method == null)
                    method = allowedMethods
                            .FirstOrDefault(m => ((DefaultCommandAttribute[])m.GetCustomAttributes(typeof(DefaultCommandAttribute), false))
                            .Any());
            }
            else if (type == MethodType.BySmile)
            {
                method = allowedMethods
                            .FirstOrDefault(m => ((EmojiCommandAttribute[])m.GetCustomAttributes(typeof(EmojiCommandAttribute), false))
                            .Any(a => message.Text.StartsWith(a.GetSmile())));

                if (method == null)
                    method = allowedMethods
                            .FirstOrDefault(m => ((EmojiDefaultCommandAttribute[])m.GetCustomAttributes(typeof(EmojiDefaultCommandAttribute), false))
                            .Any());
            }
            else if (type == MethodType.Text)
            {
                List<MethodInfo> methods = allowedMethods
                            .Where(m => ((TextCommandAttribute[])m.GetCustomAttributes(typeof(TextCommandAttribute), false))
                            .Any()).ToList();

                if (methods.Count != 0)
                {
                    string userState = await GetStateValue(message.Chat.Id);

                    if (!string.IsNullOrEmpty(userState))
                        method = methods.FirstOrDefault(m => ((StateAttribute[])m.GetCustomAttributes(typeof(StateAttribute))).Any(s => s.GetState() == userState));

                    if (method == null)
                        method = methods.FirstOrDefault(m => !m.GetCustomAttributes(typeof(StateAttribute)).Any());
                }
            }
            else if (type == MethodType.ByType)
            {
                method = allowedMethods
                     .FirstOrDefault(m => ((TypedCommandAttribute[])m.GetCustomAttributes(typeof(TypedCommandAttribute), false))
                     .Any(a => a.GetMessageType() == message.Type));
            }

            if (method != null)
                return new TelegramMethod
                {
                    ControllerType = _controllerTypes.First(c => c == method.DeclaringType),
                    Method = method
                };

            return null;
        }

        private TelegramMethod GetMethod(MethodType type, CallbackQuery callback)
        {
            MethodInfo method = null;
            string path = JsonConvert.DeserializeObject<CallbackQueryModel>(callback.Data).Path;

            foreach (Type controllerType in _controllerTypes)
            {
                if (type == MethodType.Callback)
                {
                    method = controllerType.GetMethods()
                         .FirstOrDefault(m => ((CallbackQueryAttribute[])m.GetCustomAttributes(typeof(CallbackQueryAttribute), false))
                         .Any(a => a.GetPath() == path));
                }

                if (method != null)
                    return new TelegramMethod
                    {
                        ControllerType = controllerType,
                        Method = method
                    };
            }

            return null;
        }

        private async Task<object> InvokeMethod(MethodType type, Message message, object botId = null)
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

                return method.Method.Invoke(ActivatorUtilities.CreateInstance(_provider, method.ControllerType),
                    parameters.ToArray());
            }

            return null;
        }

        private object InvokeCallback(MethodType type, CallbackQuery callback)
        {
            TelegramMethod method = GetMethod(type, callback);

            if (method.ControllerType != null && method.Method != null)
            {
                ParameterInfo[] methodParams = method.Method.GetParameters();
                List<object> parameters = new List<object> { };

                if (methodParams.Any(p => p.ParameterType == typeof(CallbackQueryData)))
                    parameters.Add(new CallbackQueryData
                    {
                        Client = _client,
                        CallbackQuery = callback,
                        BotData = _botData
                    });

                Type callbackType = typeof(CallbackQueryModel);

                if (methodParams.Any(p => p.ParameterType == callbackType))
                    parameters.Add(JsonConvert.DeserializeObject(callback.Data, callbackType));

                if (methodParams.Any(p => p.ParameterType.IsSubclassOf(callbackType)))
                    parameters.Add(JsonConvert.DeserializeObject(
                        callback.Data, methodParams.First(p => p.ParameterType.IsSubclassOf(callbackType)).ParameterType));

                return method.Method.Invoke(ActivatorUtilities.CreateInstance(_provider, method.ControllerType),
                    parameters.ToArray());
            }

            return null;
        }

        private bool IsFirstSmile(string text)
        {
            return EmojiHelper.IsStartEmoji(text);
        }

        protected MethodType GetMethodType(Message message)
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    if (message.Text.StartsWith("/"))
                        return MethodType.ByPath;
                    else if (IsFirstSmile(message.Text))
                        return MethodType.BySmile;
                    else
                        return MethodType.Text;
                default:
                    return MethodType.ByType;
            }
        }

        public async Task<object> OnMessage(MessageEventArgs e)
        {
            Message message = e.Message;

            return await InvokeMethod(GetMethodType(message), message);
        }

        public async Task<object> OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            CallbackQuery callback = e.CallbackQuery;

            if (e.CallbackQuery.Data != null)
                return InvokeCallback(MethodType.Callback, callback);

            return null;
        }
    }
}
