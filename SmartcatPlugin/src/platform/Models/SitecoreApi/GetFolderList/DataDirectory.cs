using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.GetFolderList
{
    public class DataDirectory
    {
        public ExternalObjectId Id { get; set; }
        public string Name { get; set; }
        public bool CanLoadChildDirectories { get; set; }
        public bool CanLoadChildItems { get; set; }
        public List<DataDirectory> ChildDirectories { get; set; }
    }
}