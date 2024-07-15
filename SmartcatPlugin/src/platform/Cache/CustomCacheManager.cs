using Sitecore;

namespace SmartcatPlugin.Cache
{
    public static class CustomCacheManager
    {
        private static readonly CustomCache Cache;
        static CustomCacheManager()
        {
            Cache = new CustomCache("OwnCustomCache", StringUtil.ParseSizeString("500KB"));
        }
        public static string GetCache(string key)
        {
            return Cache.GetString(key);
        }

        public static void SetCache(string key, string value)
        {
            Cache.SetString(key, value);
        }
    }
}