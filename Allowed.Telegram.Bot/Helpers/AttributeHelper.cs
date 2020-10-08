using Allowed.Telegram.Bot.Attributes;
using System;
using System.Reflection;

namespace Allowed.Telegram.Bot.Helpers
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

        public static BotNameAttribute[] GetBotNameAttributes(this Type controllerType)
        {
            return (BotNameAttribute[])controllerType.GetCustomAttributes<BotNameAttribute>(false);
        }
        
        public static CommandAttribute[] GetCommandAttributes(this MethodInfo method)
        {
            return (CommandAttribute[])method.GetCustomAttributes<CommandAttribute>(false);
        }

        public static DefaultCommandAttribute[] GetDefaultCommandAttributes(this MethodInfo method)
        {
            return (DefaultCommandAttribute[])method.GetCustomAttributes<DefaultCommandAttribute>(false);
        }

        public static EmojiCommandAttribute[] GetEmojiCommandAttributes(this MethodInfo method)
        {
            return (EmojiCommandAttribute[])method.GetCustomAttributes<EmojiCommandAttribute>(false);
        }

        public static EmojiDefaultCommandAttribute[] GetEmojiDefaultCommandAttributes(this MethodInfo method)
        {
            return (EmojiDefaultCommandAttribute[])method.GetCustomAttributes<EmojiDefaultCommandAttribute>(false);
        }

        public static TextCommandAttribute[] GetTextCommandAttributes(this MethodInfo method)
        {
            return (TextCommandAttribute[])method.GetCustomAttributes<TextCommandAttribute>(false);
        }

        public static TypedCommandAttribute[] GetTypedCommandAttributes(this MethodInfo method)
        {
            return (TypedCommandAttribute[])method.GetCustomAttributes<TypedCommandAttribute>(false);
        }

        public static CallbackQueryAttribute[] GetCallbackQueryAttributes(this MethodInfo method)
        {
            return (CallbackQueryAttribute[])method.GetCustomAttributes<CallbackQueryAttribute>(false);
        }

        public static CallbackDefaultQueryAttribute[] GetCallbackDefaultQueryAttributes(this MethodInfo method)
        {
            return (CallbackDefaultQueryAttribute[])method.GetCustomAttributes<CallbackDefaultQueryAttribute>(false);
        }

        public static InlineQueryAttribute[] GetInlineQueryAttributes(this MethodInfo method)
        {
            return (InlineQueryAttribute[])method.GetCustomAttributes<InlineQueryAttribute>(false);
        }
    }
}
