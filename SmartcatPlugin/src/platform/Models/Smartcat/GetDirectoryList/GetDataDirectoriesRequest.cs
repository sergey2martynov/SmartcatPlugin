using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Smartcat.GetDirectoryList
{
    public class GetDataDirectoriesRequest
    {
        [JsonProperty("BatchKey")]
        public string BatchKey { get; set; }
        [JsonProperty("parentDirectoryId")]
        public ExternalObjectId ParentDirectoryId { get; set; }
    }
}