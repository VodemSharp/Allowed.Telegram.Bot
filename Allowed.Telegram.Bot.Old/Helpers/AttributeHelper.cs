using System.Reflection;
using Allowed.Telegram.Bot.Attributes;

namespace Allowed.Telegram.Bot.Helpers;

public static class AttributeHelper
{
    public static BotNameAttribute[] GetBotNameAttributes(this Type controllerType)
    {
        return (BotNameAttribute[])controllerType.GetCustomAttributes<BotNameAttribute>(false);
    }

    public static IEnumerable<CommandAttribute> GetCommandAttributes(this MethodInfo method)
    {
        return (CommandAttribute[])method.GetCustomAttributes<CommandAttribute>(false);
    }

    public static IEnumerable<DefaultCommandAttribute> GetDefaultCommandAttributes(this MethodInfo method)
    {
        return (DefaultCommandAttribute[])method.GetCustomAttributes<DefaultCommandAttribute>(false);
    }

    public static IEnumerable<TextCommandAttribute> GetTextCommandAttributes(this MethodInfo method)
    {
        return (TextCommandAttribute[])method.GetCustomAttributes<TextCommandAttribute>(false);
    }

    public static IEnumerable<TypedCommandAttribute> GetTypedCommandAttributes(this MethodInfo method)
    {
        return (TypedCommandAttribute[])method.GetCustomAttributes<TypedCommandAttribute>(false);
    }

    public static IEnumerable<CallbackQueryAttribute> GetCallbackQueryAttributes(this MethodInfo method)
    {
        return (CallbackQueryAttribute[])method.GetCustomAttributes<CallbackQueryAttribute>(false);
    }

    public static IEnumerable<CallbackDefaultQueryAttribute> GetCallbackDefaultQueryAttributes(
        this MethodInfo method)
    {
        return (CallbackDefaultQueryAttribute[])method.GetCustomAttributes<CallbackDefaultQueryAttribute>(false);
    }

    public static IEnumerable<InlineQueryAttribute> GetInlineQueryAttributes(this MethodInfo method)
    {
        return (InlineQueryAttribute[])method.GetCustomAttributes<InlineQueryAttribute>(false);
    }
    
    public static IEnumerable<PreCheckoutQueryAttribute> GetPreCheckoutQueryAttributes(this MethodInfo method)
    {
        return (PreCheckoutQueryAttribute[])method.GetCustomAttributes<PreCheckoutQueryAttribute>(false);
    }
}