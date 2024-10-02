using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetFolderList
{
    public class GetFolderListResponse
    {
        public string NextBatchKey { get; set; }
        public List<DataDirectory> Directories { get; set; } = new List<DataDirectory>();
    }
}