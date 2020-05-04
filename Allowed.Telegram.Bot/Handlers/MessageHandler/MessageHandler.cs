using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Constants;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Services.Extensions.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Handlers.MessageHandler
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IControllersCollection _collection;

        public MessageHandler(IControllersCollection collection)
        {
            _collection = collection;
        }

        private (CommandController, MethodInfo) GetMethodByPath(string path)
        {
            foreach (CommandController controller in _collection.Controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .Where(m => ((CommandAttribute[])m.GetCustomAttributes(typeof(CommandAttribute), false))
                       .Any(a => $"/{a.GetPath()}" == path))
                     .FirstOrDefault();

                if (method != null)
                    return (controller, method);
            }

            return (null, null);
        }

        private (CommandController, MethodInfo) GetMethodBySmile(string text)
        {
            foreach (CommandController controller in _collection.Controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .Where(m => ((SmileCommandAttribute[])m.GetCustomAttributes(typeof(SmileCommandAttribute), false))
                       .Any(a => text.StartsWith(a.GetSmile())))
                     .FirstOrDefault();

                if (method != null)
                    return (controller, method);
            }

            return (null, null);
        }

        private (CommandController, MethodInfo) GetMethodByCallbackPath(string path)
        {
            foreach (CommandController controller in _collection.Controllers)
            {
                MethodInfo method = controller.GetType().GetMethods()
                     .Where(m => ((CallbackQueryAttribute[])m.GetCustomAttributes(typeof(CallbackQueryAttribute), false))
                       .Any(a => a.GetPath() == path))
                     .FirstOrDefault();

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
                method.Item2?.Invoke(method.Item1, new object[] { message });
            }
        }

        private void FindSmileCommand(Message message)
        {
            (CommandController, MethodInfo) method = GetMethodBySmile(message.Text);

            if (method.Item1 != null)
            {
                method.Item2?.Invoke(method.Item1, new object[] { message });
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
                    method.Item2.Invoke(method.Item1, new object[] { callback });
                else if (parameterInfos.Length == 2)
                {
                    Type type = parameterInfos[1].ParameterType;
                    var test = JsonConvert.DeserializeObject(callback.Data, type);
                    method.Item2.Invoke(method.Item1, new object[] { callback, test });
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
            try
            {
                Message message = e.Message;
                Console.WriteLine($"Received a text message in chat {message.Chat.Id}.");

                if (!string.IsNullOrEmpty(message.Text))
                {
                    if (message.Text.StartsWith("/"))
                        FindCommand(message);
                    else if (IsFirstSmile(message.Text))
                        FindSmileCommand(message);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            try
            {
                CallbackQuery callback = e.CallbackQuery;
                Message message = callback.Message;
                Console.WriteLine($"Received a text message in chat {message.Chat.Id}.");

                if (e.CallbackQuery.Data != null)
                {
                    FindCallback(callback);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
