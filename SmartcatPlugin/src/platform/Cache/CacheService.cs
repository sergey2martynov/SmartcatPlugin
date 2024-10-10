using Sitecore;
using SmartcatPlugin.Interfaces;

namespace SmartcatPlugin.Cache
{
    public class CacheService : ICacheService
    {
        private readonly CustomCache Cache;

        public CacheService()
        {
            Cache = new CustomCache("OwnCustomCache", StringUtil.ParseSizeString("500KB"));
        }

        public string GetValue(string key)
        {
            return Cache.GetValue(key);
        }

        public void SetValue(string key, string value)
        {
            Cache.SetValue(key, value);
        }
    }
}