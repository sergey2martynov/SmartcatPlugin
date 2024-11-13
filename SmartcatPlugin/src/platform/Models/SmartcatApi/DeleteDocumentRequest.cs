using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class DeleteDocumentRequest
    {
        public string WorkspaceId { get; set; }
        public string ProjectId { get; set; }
        public string DocumentId { get; set; }
    }
}