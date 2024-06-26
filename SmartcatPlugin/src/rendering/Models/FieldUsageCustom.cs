using Sitecore.LayoutService.Client.Response.Model;

namespace SmartcatPlugin.Models
{
    public class FieldUsageCustom : HeadingAndDescription
    {
        public Field<int> CustomIntField { get; set; } = default!;
    }
}
