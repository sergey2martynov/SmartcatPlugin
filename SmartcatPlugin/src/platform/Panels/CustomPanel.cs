using System.Collections.Generic;
using System.Web.UI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Interfaces;

namespace SmartcatPlugin.Panels
{
    public class CustomPanel : RibbonPanel
    {
        private readonly ICacheService _cacheService = ServiceLocator.ServiceProvider.GetService<ICacheService>();

        public override void Render(HtmlTextWriter output,
                                    Ribbon ribbon,
                                    Item button,
                                    CommandContext context)
        {
            string cachedData = _cacheService.GetValue("selectedItems");

            if (cachedData == null)
            {
                output.Write($"<div><textarea readonly>SelectedItems:0</textarea></div>");
                return;
            }

            var itemIds = JsonConvert.DeserializeObject<List<string>>(cachedData);

            var htmlOutput = $"<div><textarea readonly>SelectedItems: {itemIds.Count}</textarea></div>";

            output.Write(htmlOutput);
        }
    }
}