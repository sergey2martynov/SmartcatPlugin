using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Smartcat.GetFolderList
{
    public class GetFolderListRequest
    {
        [JsonProperty("BatchKey")]
        public string BatchKey { get; set; }
        [JsonProperty("parentDirectoryId")]
        public ExternalObjectId ParentDirectoryId { get; set; }
    }
}