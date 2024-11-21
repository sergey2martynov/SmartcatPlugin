using System.Collections.Generic;
using Smartcat.IntegrationHub.Contracts.Dto.LocJson;

namespace SmartcatPlugin.Models.SitecoreApi.GetItemContent
{
    public class GetItemContentResponse
    {
        public Dictionary<string, LocJsonContent> LocaleContent { get; set; }
    }
}