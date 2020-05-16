using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Constants;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
using Allowed.Telegram.Bot.Services.TelegramServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Handlers.MessageHandler
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ITelegramBotClient _client;
        private readonly List<CommandController> _controllers;
        private readonly BotData _botData;

        private readonly ITelegramService _telegramService;

        public MessageHandler(IControllersCollection collection, ITelegramBotClient client, BotData botData,
            ITelegramService telegramService)
        {
            _client = client;
            _botData = botData;
            _controllers = collection.Controllers
                .Where(c =>
                {
                    BotNameAttribute[] attributes = GetBotNameAttributes(c);
                    return attributes.Length == 0 || attributes.Any(a => a.GetName() == botData.Name);
                }).ToList();

            _telegramService = telegramService;
        }

        private BotNameAttribute[] GetBotNameAttributes(CommandController controller)
        {
            return (BotNameAttribute[])controller.GetType().GetCustomAttributes(typeof(BotNameAttribute), false);
        }

        private RoleAttribute[] GetRoleAttributes(CommandController controller)
        {
            return (RoleAttribute[])controller.GetType().GetCustomAttributes(typeof(RoleAttribute), false);
        }

        private RoleAttribute[] GetRoleAttributes(MethodInfo method)
        {
            return (RoleAttribute[])method.GetCustomAttributes(typeof(RoleAttribute), false);
        }

        private enum MethodType
        {
            ByPath, BySmile, ByType, Default, Callback
        }

        private MethodInfo[] GetAllowedMethods(long chatId)
        {
            if (_telegramService != null)
            {
                List<TelegramRole> userRoles = _telegramService.GetRoles(chatId).ToList();

                return _controllers.Where(c =>
                    {
                        RoleAttribute[] roles = GetRoleAttributes(c);

                        return roles.Length == 0
                            || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ur.Name));
                    })
                    .SelectMany(c => c.GetType().GetMethods().Where(m =>
                    {
                        RoleAttribute[] roles = GetRoleAttributes(m);

                        return roles.Length == 0
                            || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ur.Name));
                    })).ToArray();
            }

            return _controllers.SelectMany(c => c.GetType().GetMethods()).ToArray();
        }

        private TelegramMethod GetMethod(MethodType type, Message message)
        {
            MethodInfo method = null;
            MethodInfo[] allowedMethods = GetAllowedMethods(message.Chat.Id);

            if (type == MethodType.ByPath)
            {
                method = allowedMethods
                            .FirstOrDefault(m => ((CommandAttribute[])m.GetCustomAttributes(typeof(CommandAttribute), false))
                            .Any(a => $"/{a.GetPath()}" == message.Text));
            }
            else if (type == MethodType.BySmile)
            {
                method = allowedMethods
                            .FirstOrDefault(m => ((SmileCommandAttribute[])m.GetCustomAttributes(typeof(SmileCommandAttribute), false))
                            .Any(a => message.Text.StartsWith(a.GetSmile())));
            }
            else if (type == MethodType.Default)
            {
                method = allowedMethods
                            .FirstOrDefault(m => ((DefaultCommandAttribute[])m.GetCustomAttributes(typeof(DefaultCommandAttribute), false))
                            .Any());
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
                    Controller = _controllers.First(c => c.GetType() == method.DeclaringType),
                    Method = method
                };

            return null;
        }

        private TelegramMethod GetMethod(MethodType type, CallbackQuery callback)
        {
            MethodInfo method = null;
            string path = JsonConvert.DeserializeObject<CallbackQueryModel>(callback.Data).Path;

            foreach (CommandController controller in _controllers)
            {
                if (type == MethodType.Callback)
                {
                    method = controller.GetType().GetMethods()
                         .FirstOrDefault(m => ((CallbackQueryAttribute[])m.GetCustomAttributes(typeof(CallbackQueryAttribute), false))
                         .Any(a => a.GetPath() == path));
                }

                if (method != null)
                    return new TelegramMethod
                    {
                        Controller = controller,
                        Method = method
                    };
            }

            return null;
        }

        private void InvokeMethod(MethodType type, Message message)
        {
            TelegramMethod method = GetMethod(type, message);

            if (method != null && method.Controller != null && method.Method != null)
            {
                ParameterInfo[] methodParams = method.Method.GetParameters();
                List<object> parameters = new List<object> { };

                if (methodParams.Any(p => p.ParameterType == typeof(MessageData)))
                    parameters.Add(new MessageData
                    {
                        Message = message,
                        Client = _client,
                        BotData = _botData
                    });

                method.Method.Invoke(method.Controller, parameters.ToArray());
            }
        }

        private void InvokeCallback(MethodType type, CallbackQuery callback)
        {
            TelegramMethod method = GetMethod(type, callback);

            if (method.Controller != null && method.Method != null)
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

                method.Method.Invoke(method.Controller, parameters.ToArray());
            }
        }

        private bool IsFirstSmile(string text)
        {
            FieldInfo[] fieldInfos = typeof(CommandSmiles.People).GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            fieldInfos = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToArray();

            return fieldInfos.Any(fi => text.StartsWith((string)fi.GetValue(null)));
        }

        public void OnMessage(object sender, MessageEventArgs e)
        {
            Message message = e.Message;

            switch (message.Type)
            {
                case MessageType.Text:
                    if (message.Text.StartsWith("/"))
                        InvokeMethod(MethodType.ByPath, message);
                    else if (IsFirstSmile(message.Text))
                        InvokeMethod(MethodType.BySmile, message);
                    else
                        InvokeMethod(MethodType.Default, message);
                    break;
                default:
                    InvokeMethod(MethodType.ByType, message);
                    break;
            }
        }

        public void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            CallbackQuery callback = e.CallbackQuery;
            Message message = callback.Message;

            if (e.CallbackQuery.Data != null)
                InvokeCallback(MethodType.Callback, callback);
        }
    }
}
