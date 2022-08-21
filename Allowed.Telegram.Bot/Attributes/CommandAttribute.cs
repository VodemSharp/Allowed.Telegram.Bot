namespace Allowed.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CommandAttribute : Attribute
{
    private readonly string _path;

    public CommandAttribute(string path)
    {
        _path = path;
    }

    public string GetPath()
    {
        return _path;
    }
}