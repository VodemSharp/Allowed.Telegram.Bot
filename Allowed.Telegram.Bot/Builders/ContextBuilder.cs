using System;

namespace Allowed.Telegram.Bot.Builders
{
    public static class ContextBuilder
    {
        public static object CreateTelegramUser(Type userType, long chatId, string username)
        {
            object user = Activator.CreateInstance(userType);

            userType.GetProperty("ChatId").SetValue(user, chatId);
            userType.GetProperty("Username").SetValue(user, username);

            return user;
        }

        public static object CreateTelegramRole(Type roleType, string role)
        {
            object user = Activator.CreateInstance(roleType);

            roleType.GetProperty("Name").SetValue(user, role);

            return user;
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
