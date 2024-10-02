using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SmartcatPlugin.Models.Dtos
{
    public class ProjectListDto
    {
        [JsonProperty("projects")]
        public List<GetProjectDto> Projects { get; set; }
    }
}