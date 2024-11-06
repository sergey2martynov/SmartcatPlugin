using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;
using SmartcatPlugin.Models.SmartcatApi;

namespace SmartcatPlugin.Models.Dtos
{
    public class ProjectDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("languages")]
        public string Languages { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("isExpanded")]
        public bool IsExpanded { get; set; }

        [JsonProperty("documents")]
        public List<SmartcatDocument> Documents { get; set; }
    }
}