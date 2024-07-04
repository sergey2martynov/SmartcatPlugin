namespace SmartcatPlugin.Models.Smartcat.GetFileContent
{
    public class FileContentRequest
    {
        public ExternalObjectId ItemId { get; set; }
        public string SourceLocale { get; set; }
        public string[] TargetLocales { get; set; }
    }
}