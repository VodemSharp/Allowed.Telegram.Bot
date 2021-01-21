using System;

namespace Allowed.Telegram.Bot.Data.Helpers
{
    public class TypeHelper
    {
        public static bool IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
        {
            if (typeToCheck == typeof(object) || typeToCheck == null)
            {
                return false;
            }
            else if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        }
    }
}
