namespace SmartcatPlugin.Models.Smartcat.GetDirectoryList
{
    public class GetDataDirectoriesRequest
    {
        public string BatchKey { get; set; }

        public ExternalObjectId ParentDirectoryId { get; set; }
    }
}