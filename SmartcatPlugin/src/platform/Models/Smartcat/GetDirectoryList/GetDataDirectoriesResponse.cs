using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetDirectoryList
{
    public class GetDataDirectoriesResponse
    {
        public string NextBatchKey { get; set; }
        public List<DataDirectory> Directories { get; set; } = new List<DataDirectory>();
    }
}