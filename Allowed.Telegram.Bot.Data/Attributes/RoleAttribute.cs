using System;

namespace Allowed.Telegram.Bot.Data.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        private readonly string _role;

        public RoleAttribute(string role)
        {
            _role = role;
        }

        public string GetRole()
        {
            return _role;
        }
    }
}
