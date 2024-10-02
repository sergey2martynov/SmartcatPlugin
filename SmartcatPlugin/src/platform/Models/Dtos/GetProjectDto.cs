using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Dtos
{
    public class GetProjectDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}