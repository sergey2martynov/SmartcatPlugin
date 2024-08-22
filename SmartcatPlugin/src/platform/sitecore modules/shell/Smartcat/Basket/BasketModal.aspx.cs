using System;

namespace SmartcatPlugin.sitecore_modules.shell.Smartcat.Basket
{
    public partial class BasketModal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                Sitecore.Context.ClientPage.ClientResponse.Timer("item:refresh", 2000);
            }
        }
    }
}