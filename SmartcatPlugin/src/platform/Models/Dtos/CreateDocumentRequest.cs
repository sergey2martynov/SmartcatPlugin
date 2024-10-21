using SmartcatPlugin.Models.Smartcat;

namespace SmartcatPlugin.Models.Dtos
{
    public class CreateDocumentRequest
    {
        public string WorkSpaceId { get; set; }
        public string Title { get; set; }
        public string ProjectId { get; set; }
        public LocJsonContent Content { get; set; }
    }
}