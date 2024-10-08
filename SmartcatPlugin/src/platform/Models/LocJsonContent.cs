using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartcatPlugin.Models
{
    public class LocJsonContent
    {
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
        [JsonProperty("units")]
        public List<Unit> Units { get; set; }
    }

    public class Properties
    {
        public string ItemId { get; set; }
        public List<string> Comments { get; set; }
        public int Version { get; set; }
        public string TargetLanguage { get; set; }
    }

    public class Unit
    {
        public string Key { get; set; }
        public UnitProperties Properties { get; set; }
        public List<string> Source { get; set; }
        public List<string> Target { get; set; }
    }

    public class UnitProperties
    {
        public List<string> Comments { get; set; }

        [JsonProperty("x-smartcat-format")]
        public string SmartcatFormat { get; set; }
    }
}