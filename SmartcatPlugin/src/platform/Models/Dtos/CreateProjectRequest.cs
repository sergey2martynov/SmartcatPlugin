using System;

namespace SmartcatPlugin.Models.Dtos
{
    public class CreateProjectRequest
    {
        public string IntegrationType { get; set; }
        public string WorkspaceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public string DueDate { get; set; }
        public string ProjectTemplateId { get; set; }
    }
}