using SmartcatPlugin.Constants;

namespace SmartcatPlugin.Extensions
{
    public static class StringExtensions
    {
        public static bool IsTranslatedType(this string type)
        {
            if (!string.IsNullOrEmpty(type) && (type == ConstantItemFieldTypes.RichText 
                                                || type == ConstantItemFieldTypes.SingleLineText 
                                                || type == ConstantItemFieldTypes.MultiLineText ))
            {
                return true;
            }

            return false;
        } 
    }
}