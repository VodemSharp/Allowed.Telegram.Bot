using System.Text.Json;
using System.Text.Json.Serialization;

namespace Allowed.Telegram.Bot.Models;

public class CallbackQueryModel
{
    [JsonPropertyName("p")] public string Path { get; set; }

    public static implicit operator string(CallbackQueryModel m)
    {
        return JsonSerializer.Serialize(m, m.GetType());
    }
}