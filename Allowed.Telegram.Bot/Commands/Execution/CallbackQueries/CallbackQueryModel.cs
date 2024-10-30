using System.Text.Json;
using System.Text.Json.Serialization;

namespace Allowed.Telegram.Bot.Commands.Execution.CallbackQueries;

public class CallbackQueryModel
{
    [JsonPropertyName("p")] public virtual string Path { get; set; } = null!;

    public static implicit operator string(CallbackQueryModel m)
    {
        return JsonSerializer.Serialize(m, m.GetType());
    }
}