using Sitecore.Data;

namespace SmartcatPlugin.Extensions
{
    public static class SitecoreExtensions
    {
        public static ID GetID(this ID id, string externalId)
        {
            return new ID(externalId);
        }
    }
}