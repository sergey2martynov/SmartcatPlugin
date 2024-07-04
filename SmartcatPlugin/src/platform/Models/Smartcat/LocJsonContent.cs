using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartcatPlugin.Models.Smartcat
{
    public class LocJsonContent
    {
        public Properties Properties { get; set; }
        public List<Unit> Units { get; set; }
    }

    public class Properties
    {
        public List<string> Comments { get; set; }
        public int Version { get; set; }
    }

    public class Unit
    {
        public string Key { get; set; }
        public UnitProperties Properties { get; set; }
        public List<string> Source { get; set; }
    }

    public class UnitProperties
    {
        public List<string> Comments { get; set; }
    }
}