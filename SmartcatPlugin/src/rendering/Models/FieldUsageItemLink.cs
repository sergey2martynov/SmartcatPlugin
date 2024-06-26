using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace SmartcatPlugin.Models
{
    public class FieldUsageItemLink : HeadingAndDescription
    {
        [SitecoreComponentField(Name = "localItemLink")]
        public ItemLinkField<LinkItemTemplate> LocalItemLink { get; set; } = default!;

        [SitecoreComponentField(Name = "sharedItemLink")]
        public ItemLinkField<LinkItemTemplate> SharedItemLink { get; set; } = default!;
    }
}
