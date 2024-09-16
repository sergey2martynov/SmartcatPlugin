using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartcatPlugin.Models.Dtos
{
    public class TreeNodeDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("children")]
        public List<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();
        [JsonProperty("showCheckBox")]
        public bool ShowCheckBox { get; set; }
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }
        [JsonProperty("isChecked")]
        public bool IsChecked { get; set; }
        [JsonProperty("isExpanded")]
        public bool IsExpanded { get; set; }
    }
}