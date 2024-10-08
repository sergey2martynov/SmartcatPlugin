using Newtonsoft.Json;
using SmartcatPlugin.Models.Smartcat;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class GetItemTranslationResponse
    {
        [JsonProperty("content")]
        public LocJsonContent Content { get; set; }
        [JsonProperty("errorDetails")]
        public string ErrorDetails { get; set; }
    }
}