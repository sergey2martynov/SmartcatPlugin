using System.Collections.Generic;
using Newtonsoft.Json;
using SmartcatPlugin.Models.SmartcatApi.Base;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class GetTemplateResponse : ResponseData
    {
        [JsonProperty("templates")]
        public List<Template> Templates { get; set; }

        [JsonProperty("projectTemplatesAreEnabled")]
        public bool ProjectTemplatesAreEnabled { get; set; }
    }

    public class Template
    {
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }

        [JsonProperty("templateName")]
        public string TemplateName { get; set; }

        [JsonProperty("sourceLocales")]
        public List<string> SourceLocales { get; set; }

        [JsonProperty("targetLocales")]
        public List<string> TargetLocales { get; set; }
    }
}