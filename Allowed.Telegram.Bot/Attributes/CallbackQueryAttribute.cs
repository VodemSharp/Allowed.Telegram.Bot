namespace Allowed.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CallbackQueryAttribute : Attribute
{
    private readonly string _path;

    public CallbackQueryAttribute(string path)
    {
        _path = path;
    }

    public string GetPath()
    {
        return _path;
    }
}