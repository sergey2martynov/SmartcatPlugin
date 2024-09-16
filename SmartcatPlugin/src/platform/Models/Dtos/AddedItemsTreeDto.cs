using System.Collections.Generic;

namespace SmartcatPlugin.Models.Dtos
{
    public class AddedItemsTreeDto
    {
        public List<TreeNodeDto> TreeNodes { get; set; }
        public List<TreeNodeDto> CheckedItems { get; set; }
        public List<TreeNodeDto> ExpandedItems { get; set; }
    }
}