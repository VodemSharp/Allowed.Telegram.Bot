using Allowed.Telegram.Bot.Models;
using Newtonsoft.Json;

namespace Allowed.Telegram.Bot.Sample.Models
{
    public class TestCallbackQueryModel : CallbackQueryModel
    {
        [JsonProperty("a")]
        public bool SomeData { get; set; }
    }
}
