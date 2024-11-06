using Newtonsoft.Json;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class SmartcatDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fullPath")]
        public string FullPath { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("wordsCount")]
        public int WordsCount { get; set; }
    }
}