namespace SmartcatPlugin.Models.Smartcat.GetFileList
{
    public class GetDataItemsRequest
    {
        public string BatchKey { get; set; }
        public ExternalObjectId ParentDirectoryId { get; set; }
        public string SearchQuery { get; set; }
    }
}