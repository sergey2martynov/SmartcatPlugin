namespace SmartcatPlugin.Models.Smartcat.GetItemContent
{
    public class GetItemContentRequest
    {
        public ExternalObjectId ItemId { get; set; }
        public string SourceLocale { get; set; }
        public string[] TargetLocales { get; set; }
    }
}