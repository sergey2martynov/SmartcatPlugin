using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace SmartcatPlugin.Models
{
    public class HeadingAndDescription : HeadingOnly
    {
        public RichTextField Description { get; set; } = default!;
    }
}
