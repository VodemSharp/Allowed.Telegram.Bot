﻿using Allowed.Telegram.Bot.Models.Store;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Allowed.Telegram.Bot.Services.UserServices
{
    public interface IUserService<TKey, TUser>
        where TKey : IEquatable<TKey>
        where TUser : TelegramUser<TKey>
    {
        Task CheckUser(User user);

        Task<List<TUser>> GetUsers();
        Task<int> CountUsers();

        Task<bool> AnyUser(string username);
        Task<bool> AnyUser(long telegramId);

        Task<TUser> GetUser(string username);
        Task<TUser> GetUser(long telegramId);

        Task<TKey> GetBotUserId(long telegramId);
        Task<TKey> GetBotUserId(string username);

        Task<List<TUser>> GetUsersByRole(string role);
    }
}
