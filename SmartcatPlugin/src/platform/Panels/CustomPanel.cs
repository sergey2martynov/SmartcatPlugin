using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;

namespace SmartcatPlugin.Panels
{
    public class CustomPanel : RibbonPanel
    {
        public int SelectedItemCount { get; set; }
        private const string SelectedItems = "Seleceted items:";

        public override void Render(HtmlTextWriter output,
                                    Ribbon ribbon,
                                    Item button,
                                    CommandContext context)
        {

            Item contextItem = context.Items[0];
            Database database = Sitecore.Configuration.Factory.GetDatabase("core");

            Item item = database.SelectSingleItem(contextItem.ID.ToString());

            if (item != null)
            {
                var htmlOutput = string.Empty;

                htmlOutput += "<div><textarea>" + SelectedItems + item.DisplayName + "</textarea></div>";

                output.Write(htmlOutput);
            }
        }
    }
}