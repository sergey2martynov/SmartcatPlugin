using System;
using System.ComponentModel.DataAnnotations;

namespace SmartcatPlugin.Models.Dtos
{
    public class SaveProjectInfoDto
    {
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string WorkflowStage { get; set; }
        [Required]
        public DateTimeOffset Deadline { get; set; }
        public string Comment { get; set; }
    }
}