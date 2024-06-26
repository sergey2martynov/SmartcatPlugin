using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace SmartcatPlugin.Models
{
    public class CustomRouteType
    {
        public TextField Headline { get; set; } = default!;
        public TextField PageTitle { get; set; } = default!;
        public RichTextField Content { get; set; } = default!;
        public TextField Author { get; set; } = default!;
    }
}
