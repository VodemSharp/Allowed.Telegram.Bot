using System;

namespace Allowed.Telegram.Bot.EntityFrameworkCore.Options
{
    public class ContextOptions
    {
        public Type ContextType { get; set; }

        public Type KeyType { get; set; }
        public Type UserType { get; set; }
        public Type RoleType { get; set; }
        public Type BotType { get; set; }
        public Type BotUserType { get; set; }
        public Type BotUserRoleType { get; set; }
        public Type StateType { get; set; }
    }
}
