using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Sitecore.Annotations;

namespace SmartcatPlugin.Models.Dtos
{
    public class SaveProjectInfoDto
    {
        [Required]
        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [Required]
        [JsonProperty("workflowStages")]
        public List<string> WorkflowStages { get; set; }

        [JsonProperty("deadline")]
        public DateTimeOffset? Deadline { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}