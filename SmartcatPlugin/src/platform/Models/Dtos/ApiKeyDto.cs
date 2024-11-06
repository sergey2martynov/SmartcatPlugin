using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Dtos
{
    public class ApiKeyDto
    {
        [JsonProperty("workspaceId")]
        public string WorkspaceId { get; set; }

        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }
    }
}