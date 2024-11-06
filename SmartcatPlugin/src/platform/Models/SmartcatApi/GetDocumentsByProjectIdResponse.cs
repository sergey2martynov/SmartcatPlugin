using SmartcatPlugin.Models.SmartcatApi.Base;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class GetDocumentsByProjectIdResponse : ResponseData
    {
        [JsonProperty("documents")]
        public List<SmartcatDocument> Documents { get; set; }
    }
}