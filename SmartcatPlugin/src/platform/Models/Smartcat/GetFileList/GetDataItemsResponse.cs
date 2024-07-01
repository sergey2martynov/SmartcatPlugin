using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetFileList
{
    public class GetDataItemsResponse
    {
        public string NextBatchKey { get; set; }

        public List<DataItem> Items { get; set; } = new List<DataItem>();

        public static GetDataItemsResponse Empty => new GetDataItemsResponse()
        {
            Items = new List<DataItem>(),
            NextBatchKey = null
        };
    }
}