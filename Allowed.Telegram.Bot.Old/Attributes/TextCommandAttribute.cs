using Allowed.Telegram.Bot.Enums;

namespace Allowed.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TextCommandAttribute : Attribute
{
    private readonly string _text;
    public ComparisonTypes Type { get; set; } = ComparisonTypes.Strict;

    public TextCommandAttribute(string text = null)
    {
        _text = text;
    }

    public string GetText()
    {
        return _text;
    }
}