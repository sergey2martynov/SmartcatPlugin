using SmartcatPlugin.Models.Smartcat;

namespace SmartcatPlugin.Models.SmartcatApi
{
    public class GetItemTranslationResponse
    {
        public LocJsonContent Content { get; set; }
        public string ErrorDetails { get; set; }
    }
}