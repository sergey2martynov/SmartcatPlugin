namespace SmartcatPlugin.Models.SmartcatApi.Base
{
    public abstract class ResponseData
    {
        public virtual bool IsValid()
        {
            return true;
        }
    }
}