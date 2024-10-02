using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetItemList
{
    public class GetItemListResponse
    {
        public string NextBatchKey { get; set; }

        public List<DataItem> Items { get; set; } = new List<DataItem>();

        public static GetItemListResponse Empty => new GetItemListResponse()
        {
            Items = new List<DataItem>(),
            NextBatchKey = null
        };
    }
}