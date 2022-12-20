namespace Allowed.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TextCommandAttribute : Attribute
{
    private readonly string _text;

    public TextCommandAttribute(string text = null)
    {
        _text = text;
    }

    public string GetText()
    {
        return _text;
    }
}