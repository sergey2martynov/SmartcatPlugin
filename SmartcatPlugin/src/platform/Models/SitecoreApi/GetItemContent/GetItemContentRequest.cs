using System.Collections.Generic;
using SmartcatPlugin.Models.Smartcat;

namespace SmartcatPlugin.Models.SitecoreApi.GetItemContent
{
    public class GetItemContentRequest
    {
        public ExternalObjectId ItemId { get; set; }
        public string SourceLocale { get; set; }
        public List<string> TargetLocales { get; set; }
    }
}