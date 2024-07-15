using Sitecore.Data.Items;

namespace SmartcatPlugin.Extensions
{
    public static class ItemAppearanceExtensions
    {
        public static string GetIconPath(this ItemAppearance itemAppearance)
        {
            string icon = itemAppearance.Icon;

            if (icon.StartsWith("~"))
            {
                icon = Sitecore.StringUtil.EnsurePrefix('/', icon);
            }

            else if (!(icon.StartsWith("/") && icon.Contains(":")))
            {
                icon = Sitecore.Resources.Images.GetThemedImageSource(icon);
            }

            return icon;
        }
    }
}