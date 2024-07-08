using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetItemList
{
    public class DataItem
    {
        public ExternalObjectId Id { get; set; }
        public List<ExternalObjectId> ParentDirectoryIds { get; set; } = new List<ExternalObjectId>();

        public string Name { get; set; }
        public List<string> Locales { get; set; } = new List<string>();
    }
}