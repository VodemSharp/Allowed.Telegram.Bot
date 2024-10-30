namespace Allowed.Telegram.Bot.Exceptions;

public class AmbiguousCallbackQueryException : Exception
{
    public AmbiguousCallbackQueryException()
    {
    }

    public AmbiguousCallbackQueryException(string path)
        : base($"The specified path '{path}' is ambiguous and cannot be resolved.")
    {
    }

    public AmbiguousCallbackQueryException(string path, Exception inner)
        : base($"The specified path '{path}' is ambiguous and cannot be resolved.", inner)
    {
    }
}