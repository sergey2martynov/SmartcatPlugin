using System.Collections.Generic;

namespace SmartcatPlugin.Models.Dtos
{
    public class ItemsTreeDto
    {
        public List<TreeNodeDto> TreeNodes { get; set; }
        public List<string> CheckedItems { get; set; }
        public List<string> ExpandedItems { get; set; }
    }
}