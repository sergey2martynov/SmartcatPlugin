using Sitecore;
using SmartcatPlugin.Interfaces;

namespace SmartcatPlugin.Cache
{
    public class CacheService : ICacheService
    {
        private readonly CustomCache _cache;

        public CacheService()
        {
            _cache = new CustomCache("OwnCustomCache", StringUtil.ParseSizeString("500KB"));
        }

        public string GetValue(string key)
        {
            return _cache.GetValue(key);
        }

        public void SetValue(string key, string value)
        {
            _cache.SetValue(key, value);
        }
    }
}