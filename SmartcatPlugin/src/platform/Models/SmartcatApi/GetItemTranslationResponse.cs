using Newtonsoft.Json;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.SmartcatApi.Base;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class GetItemTranslationResponse : ResponseData
    {
        [JsonProperty("content")]
        public LocJsonContent Content { get; set; }
        [JsonProperty("errorDetails")]
        public string ErrorDetails { get; set; }

        public override bool IsValid()
        {
            if (Content == null)
            {
                return false;
            }

            return true;
        }
    }
}