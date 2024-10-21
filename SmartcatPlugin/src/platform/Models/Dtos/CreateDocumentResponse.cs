using SmartcatPlugin.Models.SmartcatApi.Base;

namespace SmartcatPlugin.Models.Dtos
{
    public class CreateDocumentResponse : ResponseData
    {
        public string DocumentId { get; set; }
    }
}