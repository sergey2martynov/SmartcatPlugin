using System.Collections.Generic;

namespace SmartcatPlugin.Models.Smartcat.Testing
{
    public class GetLocalesResponse
    {
        public string DefaultLocale { get; set; }
        public List<string> Locales { get; set; } = new List<string>();
    }
}