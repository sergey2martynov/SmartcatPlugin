using Newtonsoft.Json;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class GetExportIdRequest
    {
        [JsonProperty("workspaceId")]
        public string WorkspaceId { get; set; }
        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
        [JsonProperty("documentId")]
        public string DocumentId { get; set; }
    }
}