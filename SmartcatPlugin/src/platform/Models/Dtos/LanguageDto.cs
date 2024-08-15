using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Dtos
{
    public class LanguageDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}