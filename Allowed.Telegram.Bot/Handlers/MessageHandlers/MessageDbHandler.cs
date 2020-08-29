using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Factories.ServiceFactories;
using Allowed.Telegram.Bot.Helpers;
using Allowed.Telegram.Bot.Models;
using Allowed.Telegram.Bot.Models.Enums;
using Allowed.Telegram.Bot.Models.Store;
using Allowed.Telegram.Bot.Services.RoleServices;
using Allowed.Telegram.Bot.Services.StateServices;
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

namespace Allowed.Telegram.Bot.Handlers.MessageHandlers
{
    public class MessageDbHandler<TKey, TRole, TState> : MessageHandler

        where TKey : IEquatable<TKey>
        where TRole : TelegramRole<TKey>
        where TState : TelegramState<TKey>
    {
        private readonly IRoleService<TKey, TRole> _roleService;
        private readonly IStateService<TKey, TState> _stateService;

        public MessageDbHandler(ControllersCollection collection, ITelegramBotClient client, BotData botData,
            IRoleService<TKey, TRole> roleService, IStateService<TKey, TState> stateService, IServiceProvider provider)
            : base(collection, client, botData, provider)
        {
            _roleService = roleService;
            _stateService = stateService;
        }

        protected override async Task<string> GetStateValue(long chatId)
        {
            return (await _stateService.GetState(chatId))?.Value;
        }

        protected override async Task<MethodInfo[]> GetAllowedMethods(long chatId)
        {
            List<TRole> userRoles = await _roleService.GetUserRoles(chatId);
            string state = await GetStateValue(chatId);

            return _controllerTypes.Where(c =>
            {
                RoleAttribute[] roles = c.GetRoleAttributes();
                StateAttribute[] states = c.GetStateAttributes();

                return (roles.Length == 0 || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ur.Name)))
                && (states.Length == 0 || states.Any(s => s.GetState() == state));
            })
                       .SelectMany(c => c.GetMethods().Where(m =>
                         {
                             RoleAttribute[] roles = m.GetRoleAttributes();
                             StateAttribute[] states = m.GetStateAttributes();

                             return (roles.Length == 0 || userRoles.Any(ur => roles.Select(r => r.GetRole()).Contains(ur.Name)))
                             && (states.Length == 0 || states.Any(s => s.GetState() == state));
                         })).ToArray();
        }

        private async Task<object> InvokeMethod(MethodType type, Message message, TKey botId)
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

                CommandController<TKey> controller = (CommandController<TKey>)ActivatorUtilities
                    .CreateInstance(_provider, method.ControllerType);

                controller.BotId = botId;
                controller.Initialize(_provider.GetService<IServiceFactory>());

                return method.Method.Invoke(controller, parameters.ToArray());
            }

            return null;
        }

        private async Task<object> InvokeCallback(MethodType type, CallbackQuery callback, TKey botId)
        {
            TelegramMethod method = await GetMethod(type, callback);

            if (method.ControllerType != null && method.Method != null)
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

                CommandController<TKey> controller = (CommandController<TKey>)ActivatorUtilities
                    .CreateInstance(_provider, method.ControllerType);

                controller.BotId = botId;
                controller.Initialize(_provider.GetService<IServiceFactory>());

                return method.Method.Invoke(controller, parameters.ToArray());
            }

            return null;
        }

        public async Task<object> OnMessage(MessageEventArgs e, TKey botId)
        {
            Message message = e.Message;

            return await InvokeMethod(GetMethodType(message), message, botId);
        }

        public async Task<object> OnCallbackQuery(CallbackQueryEventArgs e, TKey botId)
        {
            CallbackQuery callback = e.CallbackQuery;

            if (e.CallbackQuery.Data != null)
                return await InvokeCallback(MethodType.Callback, callback, botId);

            return null;
        }
    }
}
