namespace SmartcatPlugin.Models.Smartcat.GetItemList
{
    public class GetItemsListRequest
    {
        public string BatchKey { get; set; }
        public ExternalObjectId ParentDirectoryId { get; set; }
        public string SearchQuery { get; set; }
    }
}