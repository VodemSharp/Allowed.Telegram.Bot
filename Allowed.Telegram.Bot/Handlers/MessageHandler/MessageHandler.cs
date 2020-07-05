using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.StateServices;
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
    public class MessageHandler<TRole, TState> : IMessageHandler

        where TRole : class
        where TState : class
    {
        private readonly ITelegramBotClient _client;
        private readonly List<CommandController> _controllers;
        private readonly BotData _botData;

        private readonly IRoleService<TRole> _roleService;
        private readonly IStateService<TState> _stateService;

        public MessageHandler(IControllersCollection collection, ITelegramBotClient client, BotData botData,
            IRoleService<TRole> roleService, IStateService<TState> stateService)
        {
            _client = client;
            _botData = botData;
            _controllers = collection.Controllers
                .Where(c =>
                {
                    BotNameAttribute[] attributes = GetBotNameAttributes(c);
                    return attributes.Length == 0 || attributes.Any(a => a.GetName() == botData.Name);
                }).ToList();

            _roleService = roleService;
            _stateService = stateService;
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

        private StateAttribute[] GetStateAttributes(CommandController controller)
        {
            return (StateAttribute[])controller.GetType().GetCustomAttributes(typeof(StateAttribute), false);
        }

        private StateAttribute[] GetStateAttributes(MethodInfo method)
        {
            return (StateAttribute[])method.GetCustomAttributes(typeof(StateAttribute), false);
        }

        private enum MethodType
        {
            ByPath, BySmile, ByType, Text, Callback
        }

        private string GetStateValue(TState userState)
        {
            return userState == null ? null : (string)ReflectionHelper.GetProperty(userState, "Value");
        }

        private MethodInfo[] GetAllowedMethods(List<TRole> userRoles = null, TState userState = null)
        {
            if (userState != null)
            {
                return _controllers.Where(c =>
                    {
                        RoleAttribute[] roles = GetRoleAttributes(c);
                        StateAttribute[] states = GetStateAttributes(c);

                        return (roles.Length == 0 || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ReflectionHelper.GetProperty(ur, "Name"))))
                        && (states.Length == 0 || states.Any(s => s.GetState() == GetStateValue(userState)));
                    })
                    .SelectMany(c => c.GetType().GetMethods().Where(m =>
                    {
                        RoleAttribute[] roles = GetRoleAttributes(m);
                        StateAttribute[] states = GetStateAttributes(m);

                        return (roles.Length == 0 || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ReflectionHelper.GetProperty(ur, "Name"))))
                        && (states.Length == 0 || states.Any(s => s.GetState() == GetStateValue(userState)));
                    })).ToArray();
            }

            return _controllers.SelectMany(c => c.GetType().GetMethods()).ToArray();
        }

        private TelegramMethod GetMethod(MethodType type, Message message)
        {
            MethodInfo method = null;
            MethodInfo[] allowedMethods;

            List<TRole> userRoles = null;
            TState userState = null;


            if (_roleService != null && _stateService != null)
            {
                userRoles = _roleService.GetRoles(message.Chat.Id).ToList();
                userState = _stateService.GetState(message.Chat.Id);
            }
            else
            {
                userRoles = new List<TRole> { };
            }

            allowedMethods = GetAllowedMethods(userRoles, userState);

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
                    method = methods.FirstOrDefault(m => ((StateAttribute[])m.GetCustomAttributes(typeof(StateAttribute))).Any(s => s.GetState() == GetStateValue(userState)));

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
            return EmojiHelper.IsStartEmoji(text);
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
                        InvokeMethod(MethodType.Text, message);
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
