using Allowed.Telegram.Bot.Enums;

namespace Allowed.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CommandAttribute : Attribute
{
    private readonly string _path;
    public ComparisonTypes Type { get; set; } = ComparisonTypes.Parameterized;

    public CommandAttribute(string path)
    {
        _path = path;
    }

    public string GetPath()
    {
        return _path;
    }
}