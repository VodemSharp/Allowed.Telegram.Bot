using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Constants;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
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

        public MessageHandler(IControllersCollection collection, ITelegramBotClient client, BotData botData)
        {
            _client = client;
            _botData = botData;
            _controllers = collection.Controllers
                .Where(c =>
                {
                    BotNameAttribute[] attributes = GetBotNameAttributes(c);
                    return attributes.Length == 0 || attributes.Any(a => a.GetName() == botData.Name);
                }).ToList();
        }

        private BotNameAttribute[] GetBotNameAttributes(CommandController controller)
        {
            return ((BotNameAttribute[])controller.GetType().GetCustomAttributes(typeof(BotNameAttribute), false));
        }

        private (CommandController, MethodInfo) GetMethodByPath(string path)
        {
            foreach (CommandController controller in _controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .FirstOrDefault(m => ((CommandAttribute[])m.GetCustomAttributes(typeof(CommandAttribute), false))
                       .Any(a => $"/{a.GetPath()}" == path));

                if (method != null)
                    return (controller, method);
            }

            return (null, null);
        }

        private (CommandController, MethodInfo) GetDefaultMethod()
        {
            foreach (CommandController controller in _controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .FirstOrDefault(m => ((DefaultCommandAttribute[])m.GetCustomAttributes(typeof(DefaultCommandAttribute), false)).Any());

                if (method != null)
                    return (controller, method);
            }

            return (null, null);
        }

        private (CommandController, MethodInfo) GetMethodBySmile(string text)
        {
            foreach (CommandController controller in _controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .FirstOrDefault(m => ((SmileCommandAttribute[])m.GetCustomAttributes(typeof(SmileCommandAttribute), false))
                       .Any(a => text.StartsWith(a.GetSmile())));

                if (method != null)
                    return (controller, method);
            }

            return (null, null);
        }

        private (CommandController, MethodInfo) GetMethodByType(MessageType type)
        {
            foreach (CommandController controller in _controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .FirstOrDefault(m => ((TypedCommandAttribute[])m.GetCustomAttributes(typeof(TypedCommandAttribute), false))
                     .Any(a => a.GetMessageType() == type));

                if (method != null)
                    return (controller, method);
            }

            return (null, null);
        }

        private (CommandController, MethodInfo) GetMethodByCallbackPath(string path)
        {
            foreach (CommandController controller in _controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .FirstOrDefault(m => ((CallbackQueryAttribute[])m.GetCustomAttributes(typeof(CallbackQueryAttribute), false))
                       .Any(a => a.GetPath() == path));

                if (method != null)
                    return (controller, method);
            }

            return (null, null);
        }

        private void FindCommand(Message message)
        {
            (CommandController, MethodInfo) method = GetMethodByPath(message.Text);

            if (method.Item1 != null)
            {
                method.Item2?.Invoke(method.Item1, new object[] {
                    new MessageData {
                        Message = message,
                        Client = _client,
                        BotData = _botData
                    }
                });
            }
        }

        private void FindDefaultCommand(Message message)
        {
            (CommandController, MethodInfo) method = GetDefaultMethod();

            if (method.Item1 != null)
            {
                method.Item2?.Invoke(method.Item1, new object[] {
                    new MessageData {
                        Message = message,
                        Client = _client,
                        BotData = _botData
                    }
                });
            }
        }

        private void FindSmileCommand(Message message)
        {
            (CommandController, MethodInfo) method = GetMethodBySmile(message.Text);

            if (method.Item1 != null)
            {
                method.Item2?.Invoke(method.Item1, new object[] {
                    new MessageData {
                        Message = message,
                        Client = _client,
                        BotData = _botData
                    }
                });
            }
        }

        private void FindTypedCommand(Message message)
        {
            (CommandController, MethodInfo) method = GetMethodByType(message.Type);

            if (method.Item1 != null)
            {
                method.Item2?.Invoke(method.Item1, new object[] {
                    new MessageData {
                        Message = message,
                        Client = _client,
                        BotData = _botData
                    }
                });
            }
        }

        private void FindCallback(CallbackQuery callback)
        {
            (CommandController, MethodInfo) method
                = GetMethodByCallbackPath(JsonConvert.DeserializeObject<CallbackQueryModel>(callback.Data).Path);

            if (method.Item1 != null && method.Item2 != null)
            {
                ParameterInfo[] parameterInfos = method.Item2.GetParameters();

                if (parameterInfos.Length == 1)
                    method.Item2.Invoke(method.Item1, new object[] {
                        new CallbackQueryData {
                            Client = _client,
                            CallbackQuery = callback,
                            BotData = _botData
                        }
                    });
                else if (parameterInfos.Length == 2)
                {
                    Type type = parameterInfos[1].ParameterType;
                    CallbackQueryModel model = (CallbackQueryModel)JsonConvert.DeserializeObject(callback.Data, type);
                    method.Item2.Invoke(method.Item1, new object[] {
                        new CallbackQueryData
                        {
                            Client = _client,
                            CallbackQuery = callback,
                            BotData = _botData
                        },
                        model
                    });
                }
            }
        }

        private bool IsFirstSmile(string text)
        {
            FieldInfo[] fieldInfos = typeof(CommandSmiles.People).GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            fieldInfos = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToArray();

            return fieldInfos.Any(fi => text.StartsWith((string)fi.GetValue(null)));
        }

        public async void OnMessage(object sender, MessageEventArgs e)
        {
            Message message = e.Message;

            switch (message.Type)
            {
                case MessageType.Text:
                    if (message.Text.StartsWith("/"))
                        FindCommand(message);
                    else if (IsFirstSmile(message.Text))
                        FindSmileCommand(message);
                    else
                        FindDefaultCommand(message);
                    break;
                default:
                    FindTypedCommand(message);
                    break;
            }
        }

        public async void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            CallbackQuery callback = e.CallbackQuery;
            Message message = callback.Message;

            if (e.CallbackQuery.Data != null)
            {
                FindCallback(callback);
            }
        }
    }
}
