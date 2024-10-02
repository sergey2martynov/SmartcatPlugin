using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetParentDirectories
{
    public class GetParentFoldersRequest
    {
        [JsonProperty("directoryIds")]
        public List<ExternalObjectId> DirectoryIds { get; set; } = new List<ExternalObjectId>();
    }
}