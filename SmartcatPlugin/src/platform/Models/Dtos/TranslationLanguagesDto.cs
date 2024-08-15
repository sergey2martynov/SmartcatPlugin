using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartcatPlugin.Models.Dtos
{
    public class TranslationLanguagesDto
    {
        [JsonProperty("sourceLanguages")]
        public List<LanguageDto> SourceLanguages { get; set; }
        [JsonProperty("targetLanguages")]
        public List<LanguageDto> TargetLanguages { get; set; }
    }
}