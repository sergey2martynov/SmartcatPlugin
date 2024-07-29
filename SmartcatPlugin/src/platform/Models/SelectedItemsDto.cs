using System.Collections.Generic;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Models
{
    public class SelectedItemsDto
    {
        public List<TreeNodeDto> TreeNodes { get; set; }
        public List<string> CheckedItems { get; set; }
        public List<string> ExpandedItems { get; set; }
    }
}