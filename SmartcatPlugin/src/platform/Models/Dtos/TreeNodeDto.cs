using System.Collections.Generic;

namespace SmartcatPlugin.Models.Dtos
{
    public class TreeNodeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();
        public bool ShowCheckbox { get; set; }
        public string ImageUrl { get; set; }
    }
}