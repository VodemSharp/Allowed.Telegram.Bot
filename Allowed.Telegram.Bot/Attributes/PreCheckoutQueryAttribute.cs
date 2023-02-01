namespace Allowed.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class PreCheckoutQueryAttribute : Attribute
{
}