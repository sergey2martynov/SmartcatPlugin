using System.Collections.Generic;
using Newtonsoft.Json;
using SmartcatPlugin.Models.SmartcatApi.Base;

namespace SmartcatPlugin.Models.Dtos
{
    public class GetProjectListResponse : ResponseData
    {
        [JsonProperty("projects")]
        public List<GetProjectDto> Projects { get; set; }
    }
}