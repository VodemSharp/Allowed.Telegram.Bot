using Allowed.Telegram.Bot.Data.Models;
using System;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Builders
{
    public static class ContextBuilder
    {
        public static TUser CreateTelegramUser<TKey, TUser>(long telegramId, string username,
                string firstName, string lastName, string languageCode)
            where TKey : IEquatable<TKey>
            where TUser : TelegramUser<TKey>
        {
            TUser user = Activator.CreateInstance<TUser>();

            user.TelegramId = telegramId;
            user.Username = username;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.LanguageCode = languageCode;

            return user;
        }

        public static TRole CreateTelegramRole<TKey, TRole>(string roleName)
            where TKey : IEquatable<TKey>
            where TRole : TelegramRole<TKey>
        {
            TRole role = Activator.CreateInstance<TRole>();

            role.Name = roleName;

            return role;
        }

        public static TBot CreateTelegramBot<TKey, TBot>(string botName)
            where TKey : IEquatable<TKey>
            where TBot : TelegramBot<TKey>
        {
            TBot bot = Activator.CreateInstance<TBot>();

            bot.Name = botName;

            return bot;
        }

        public static TBotUser CreateTelegramBotUser<TKey, TBotUser>(TKey userId, TKey botId)
            where TKey : IEquatable<TKey>
            where TBotUser : TelegramBotUser<TKey>
        {
            TBotUser botUser = Activator.CreateInstance<TBotUser>();

            botUser.TelegramUserId = userId;
            botUser.TelegramBotId = botId;

            return botUser;
        }

        public static object CreateTelegramBotUser<TKey>(Type botUserType, TKey userId, TKey botId)
            where TKey : IEquatable<TKey>
        {
            object botUser = Activator.CreateInstance(botUserType);

            botUserType.GetProperty("TelegramUserId").SetValue(botUser, userId);
            botUserType.GetProperty("TelegramBotId").SetValue(botUser, botId);

            return botUser;
        }

        public static TBotUserRole CreateTelegramBotUserRole<TKey, TBotUserRole>(TKey botUserId, TKey roleId)
            where TKey : IEquatable<TKey>
            where TBotUserRole : TelegramBotUserRole<TKey>
        {
            TBotUserRole userRole = Activator.CreateInstance<TBotUserRole>();

            userRole.TelegramBotUserId = botUserId;
            userRole.TelegramRoleId = roleId;

            return userRole;
        }

        public static object CreateTelegramBotUserRole<TKey>(Type botUserRoleType, TKey botUserId, TKey roleId)
            where TKey : IEquatable<TKey>
        {
            object botUserRole = Activator.CreateInstance(botUserRoleType);

            botUserRoleType.GetProperty("TelegramBotUserId").SetValue(botUserRole, botUserId);
            botUserRoleType.GetProperty("TelegramRoleId").SetValue(botUserRole, roleId);

            return botUserRole;
        }
    }
}
