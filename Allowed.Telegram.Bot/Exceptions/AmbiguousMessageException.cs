using Telegram.Bot.Types.Enums;

namespace Allowed.Telegram.Bot.Exceptions;

public class AmbiguousMessageException : Exception
{
    public AmbiguousMessageException()
    {
    }

    public AmbiguousMessageException(string? message, MessageType type)
        : base($"The specified message '{message}' with type '{type}' is ambiguous and cannot be resolved.")
    {
    }

    public AmbiguousMessageException(string? message, MessageType type, Exception inner)
        : base($"The specified message '{message}' with type '{type}' is ambiguous and cannot be resolved.", inner)
    {
    }
}