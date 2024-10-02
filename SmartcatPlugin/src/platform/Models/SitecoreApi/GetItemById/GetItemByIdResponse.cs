using SmartcatPlugin.Models.Smartcat.GetItemList;
using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetItemById
{
    public class GetItemByIdResponse
    {
        public List<DataItem> Items { get; set; } = new List<DataItem>();
    }
}