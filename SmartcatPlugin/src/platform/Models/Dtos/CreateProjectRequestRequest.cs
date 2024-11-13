using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Dtos
{
    public class CreateProjectRequestRequest
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

        [JsonProperty("targetLanguage")]
        public string TargetLanguage { get; set; }

        [JsonProperty("dueDate")]
        public string DueDate { get; set; }

        [JsonProperty("projectTemplateId")]
        public string ProjectTemplateId { get; set; }

        [JsonProperty("selectedItemIds")]
        public List<string> SelectedItemIds { get; set; }
    }
}