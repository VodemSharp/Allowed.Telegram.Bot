using System.Text.Json.Serialization;
using Allowed.Telegram.Bot.Models;

namespace Allowed.Telegram.Bot.Sample.Models;

public class TestCallbackQueryModel : CallbackQueryModel
{
    [JsonPropertyName("a")] public bool SomeData { get; set; }
}