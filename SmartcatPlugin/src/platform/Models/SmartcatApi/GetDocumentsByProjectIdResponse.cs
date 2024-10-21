using SmartcatPlugin.Models.SmartcatApi.Base;
using System.Collections.Generic;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class GetDocumentsByProjectIdResponse : ResponseData
    {
        public List<SmartcatDocument> Documents { get; set; }
    }
}