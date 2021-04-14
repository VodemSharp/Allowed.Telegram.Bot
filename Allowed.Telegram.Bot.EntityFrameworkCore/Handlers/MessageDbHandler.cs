using Allowed.Telegram.Bot.Data.Attributes;
using Allowed.Telegram.Bot.Data.Controllers;
using Allowed.Telegram.Bot.Data.Factories;
using Allowed.Telegram.Bot.Data.Helpers;
using Allowed.Telegram.Bot.Data.Models;
using Allowed.Telegram.Bot.Data.Services;
using Allowed.Telegram.Bot.EntityFrameworkCore.Middlewares;
using Allowed.Telegram.Bot.Extensions.Collections;
using Allowed.Telegram.Bot.Handlers;
using Allowed.Telegram.Bot.Helpers;
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

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Handlers
{
    public class MessageDbHandler<TKey, TUser, TRole> : MessageHandler

        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
        where TRole : TelegramRole<TKey>
    {
        private readonly IUserService<TKey, TUser> _userService;
        private readonly IRoleService<TKey, TRole> _roleService;

        public MessageDbHandler(ControllersCollection collection, ITelegramBotClient client, BotData botData,
            IUserService<TKey, TUser> userService, IRoleService<TKey, TRole> roleService, IServiceProvider provider)
            : base(collection, client, botData, provider)
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
            List<TRole> userRoles = await _roleService.GetUserRoles(telegramId);
            string state = await GetStateValue(telegramId);

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

        protected override async Task<MethodInfo> GetMethodByText(MethodInfo[] methods, Message message)
        {
            List<MethodInfo> textMethods = methods.Where(m => m.GetTextCommandAttributes().Any()).ToList();
            MethodInfo method = null;

            if (textMethods.Count != 0)
            {
                string userState = await GetStateValue(message.From.Id);

                if (!string.IsNullOrEmpty(userState))
                    method = textMethods.FirstOrDefault(m => m.GetStateAttributes().Any(s => s.GetState() == userState));

                if (method == null)
                    method = textMethods.FirstOrDefault(m => !m.GetStateAttributes().Any());
            }

            return method;
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

                IServiceFactory serviceFactory = _provider.GetService<IServiceFactory>();

                controller.Initialize(serviceFactory, message.From.Id);
                await controller.InitializeAsync(serviceFactory, message.From.Id);

                return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
            }

            return null;
        }

        private async Task<object> InvokeCallback(MethodType type, CallbackQuery callback, TKey botId)
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

                CommandController<TKey> controller = (CommandController<TKey>)ActivatorUtilities
                    .CreateInstance(_provider, method.ControllerType);

                controller.BotId = botId;

                IServiceFactory serviceFactory = _provider.GetService<IServiceFactory>();

                controller.Initialize(serviceFactory, callback.From.Id);
                await controller.InitializeAsync(serviceFactory, callback.From.Id);

                return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
            }

            return null;
        }

        private async Task<object> InvokeInline(MethodType type, InlineQuery inline, TKey botId)
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

                CommandController<TKey> controller = (CommandController<TKey>)ActivatorUtilities
                    .CreateInstance(_provider, method.ControllerType);

                controller.BotId = botId;

                IServiceFactory serviceFactory = _provider.GetService<IServiceFactory>();

                controller.Initialize(serviceFactory, inline.From.Id);
                await controller.InitializeAsync(serviceFactory, inline.From.Id);

                return await MethodHelper.InvokeMethod(method.Method, parameters, controller);
            }

            return null;
        }

        public async Task<object> OnMessage(MessageEventArgs e, TKey botId)
        {
            Message message = e.Message;
            object result = null;

            try
            {
                result = await InvokeMethod(GetMethodType(message), message, botId);

                MessageDbMiddleware<TKey> messageMiddleware = _provider.GetService<MessageDbMiddleware<TKey>>();
                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterMessageProcessed(botId, e.Message.From.Id);
                    await messageMiddleware.AfterMessageProcessedAsync(botId, e.Message.From.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }

        public async Task<object> OnCallbackQuery(CallbackQueryEventArgs e, TKey botId)
        {
            CallbackQuery callback = e.CallbackQuery;
            object result = null;

            try
            {
                if (e.CallbackQuery.Data != null)
                    result = await InvokeCallback(MethodType.Callback, callback, botId);

                MessageDbMiddleware<TKey> messageMiddleware = _provider.GetService<MessageDbMiddleware<TKey>>();
                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterCallbackProcessed(botId, e.CallbackQuery.From.Id);
                    await messageMiddleware.AfterCallbackProcessedAsync(botId, e.CallbackQuery.From.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }

        public async Task<object> OnInlineQuery(InlineQueryEventArgs e, TKey botId)
        {
            InlineQuery inline = e.InlineQuery;
            object result = null;

            try
            {
                result = await InvokeInline(MethodType.Inline, inline, botId);

                MessageDbMiddleware<TKey> messageMiddleware = _provider.GetService<MessageDbMiddleware<TKey>>();
                if (messageMiddleware != null)
                {
                    messageMiddleware.AfterInlineProcessed(botId, inline.From.Id);
                    await messageMiddleware.AfterInlineProcessedAsync(botId, inline.From.Id);
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
