using Smartcat.IntegrationHub.Contracts.Dto.LocJson;
using Smartcat.IntegrationHub.Contracts.Dto.Shared;
namespace SmartcatPlugin.Models.Dtos
{
    public class CreateDocumentRequest
    {
        public string WorkSpaceId { get; set; }
        public string Title { get; set; }
        public string ProjectId { get; set; }
        public ExternalObjectId? ExternalObjectId { get; set; }
        public LocJsonContent? Content { get; set; }
        public string TargetLanguage { get; set; }
    }
}