using System.Text.Json.Serialization;

namespace Allowed.Telegram.Bot.Models;

public class CallbackQueryModel
{
    [JsonPropertyName("p")] public string Path { get; set; }
}