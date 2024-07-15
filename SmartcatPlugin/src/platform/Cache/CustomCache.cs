namespace SmartcatPlugin.Cache
{
    public class CustomCache : Sitecore.Caching.CustomCache
    {
        public CustomCache(string name, long maxSize) : base(name, maxSize)
        {

        }
        public new void SetString(string key, string value)
        {
            base.SetString(key, value);
        }
        public new string GetString(string key)
        {
            return base.GetString(key);
        }
    }
}