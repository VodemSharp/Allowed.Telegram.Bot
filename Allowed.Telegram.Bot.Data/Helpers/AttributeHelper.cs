using Allowed.Telegram.Bot.Data.Attributes;
using System;
using System.Reflection;

namespace Allowed.Telegram.Bot.Data.Helpers
{
    public static class AttributeHelper
    {
        public static RoleAttribute[] GetRoleAttributes(this Type controllerType)
        {
            return (RoleAttribute[])controllerType.GetCustomAttributes<RoleAttribute>(false);
        }

        public static RoleAttribute[] GetRoleAttributes(this MethodInfo method)
        {
            return (RoleAttribute[])method.GetCustomAttributes<RoleAttribute>(false);
        }

        public static StateAttribute[] GetStateAttributes(this Type controllerType)
        {
            return (StateAttribute[])controllerType.GetCustomAttributes<StateAttribute>(false);
        }

        public static StateAttribute[] GetStateAttributes(this MethodInfo method)
        {
            return (StateAttribute[])method.GetCustomAttributes<StateAttribute>(false);
        }
    }
}
