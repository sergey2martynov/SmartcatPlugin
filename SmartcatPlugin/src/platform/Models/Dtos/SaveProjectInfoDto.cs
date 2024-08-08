﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SmartcatPlugin.Models.Dtos
{
    public class SaveProjectInfoDto
    {
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string WorkflowStage { get; set; }
        [Required]
        public DateTimeOffset Deadline { get; set; }
        public string Description { get; set; }
    }
}