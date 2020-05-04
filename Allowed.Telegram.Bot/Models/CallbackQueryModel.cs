using Newtonsoft.Json;

namespace Allowed.Telegram.Bot.Models
{
    public class CallbackQueryModel
    {
        [JsonProperty("p")]
        public string Path { get; set; }
    }
}
