using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.ImportTranslation
{
    public class TranslationImportRequest
    {
        public ExternalObjectId ItemId { get; set; }
        public Dictionary<string, LocJsonContent> LocaleContent { get; set; }
    }
}