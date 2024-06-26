using System.Collections.Generic;
using Sitecore.Data;

namespace SmartcatPlugin.Models
{
    public class PageModel
    {
        public ID Id { get; set; }
        public string Name { get; set; }
        public ItemUri Url { get; set; }
        public List<ItemField> Items { get; set; }
        public IEnumerable<ItemField> Fields { get; set; }
    }
}