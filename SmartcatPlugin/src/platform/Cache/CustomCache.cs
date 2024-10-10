namespace SmartcatPlugin.Cache
{
    public class CustomCache : Sitecore.Caching.CustomCache
    {
        public CustomCache(string name, long maxSize) : base(name, maxSize)
        {

        }

        public void SetValue(string key, string value)
        {
            SetString(key, value);
        }

        public string GetValue(string key)
        {
            return GetString(key);
        }
    }
}