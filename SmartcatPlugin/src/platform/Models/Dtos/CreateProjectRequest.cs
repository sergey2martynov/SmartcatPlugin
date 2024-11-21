using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Dtos
{
    public class CreateProjectRequest
    {
        [JsonProperty("integrationType")]
        public string IntegrationType { get; set; }

        [JsonProperty("workspaceId")]
        public string WorkspaceId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("sourceLanguage")]
        public string SourceLanguage { get; set; }

        [JsonProperty("targetLanguages")]
        public List<string> TargetLanguages { get; set; }

        [JsonProperty("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonProperty("projectTemplateId")]
        public Guid? ProjectTemplateId { get; set; }

        [JsonProperty("selectedItemIds")]
        public List<string> SelectedItemIds { get; set; }
        [JsonProperty("stage")]
        public int Stage { get; set; }
    }
}