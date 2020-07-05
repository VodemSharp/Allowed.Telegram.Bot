using System;

namespace Allowed.Telegram.Bot.Options
{
    public class ContextOptions
    {
        public Type ContextType { get; set; }

        public Type UserType { get; set; }
        public Type RoleType { get; set; }
        public Type UserRoleType { get; set; }
        public Type StateType { get; set; }
        public Type BotType { get; set; }
    }
}
