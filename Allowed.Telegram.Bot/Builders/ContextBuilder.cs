using Allowed.Telegram.Bot.Helpers;
using System;

namespace Allowed.Telegram.Bot.Builders
{
    public static class ContextBuilder
    {
        public static object CreateTelegramUser(Type userType, long chatId, string username)
        {
            object user = Activator.CreateInstance(userType);

            user.SetProperty("ChatId", chatId);
            user.SetProperty("Username", username);

            return user;
        }

        public static object CreateTelegramRole(Type roleType, string role)
        {
            object user = Activator.CreateInstance(roleType);

            roleType.GetProperty("Name").SetValue(user, role);

            return user;
        }

        public static object CreateTelegramUserRole(Type userRoleType, object userId, object roleId)
        {
            object userRole = Activator.CreateInstance(userRoleType);

            userRole.SetProperty("UserId", userId);
            userRole.SetProperty("RoleId", roleId);

            return userRole;
        }

        public static object CreateTelegramState(Type stateType, object userId, object botId, string value)
        {
            object state = Activator.CreateInstance(stateType);

            stateType.GetProperty("TelegramUserId").SetValue(state, userId);
            stateType.GetProperty("TelegramBotId").SetValue(state, botId);
            stateType.GetProperty("Value").SetValue(state, value);

            return state;
        }
    }
}
