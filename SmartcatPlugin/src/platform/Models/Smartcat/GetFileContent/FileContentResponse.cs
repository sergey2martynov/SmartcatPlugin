using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetFileContent
{
    public class FileContentResponse
    {
        public Dictionary<string, LocJsonContent> LocaleContent { get; set; }
    }
}