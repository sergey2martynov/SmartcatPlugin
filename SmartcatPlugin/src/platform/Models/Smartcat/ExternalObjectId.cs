using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Smartcat
{
    public class ExternalObjectId
    {
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("externalType")]
        public string ExternalType { get; set; }

        public static ExternalObjectId Empty = new ExternalObjectId();
    }
}