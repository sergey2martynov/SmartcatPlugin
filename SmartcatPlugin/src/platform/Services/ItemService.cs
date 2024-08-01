using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Data.Items;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Services
{
    public class ItemService
    {
        public ItemsTreeDto GetContentEditorItemsTree()
        {
            var masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            var rootItem = masterDb.GetItem("/sitecore/content");
            string cachedData = CustomCacheManager.GetCache("selectedItems");
            var selectedItemIds = cachedData == null ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(cachedData);

            var rootNode = new TreeNodeDto
            {
                Id = rootItem.ID.ToString(),
                Name = rootItem.Name,
                ShowCheckBox = false,
                ImageUrl = rootItem.Appearance.GetIconPath()
            };
            AddChildNodes(rootItem, rootNode);

            var result = new ItemsTreeDto
            {
                TreeNodes = new List<TreeNodeDto> { rootNode },
                CheckedItems = selectedItemIds ?? new List<string>(),
                ExpandedItems = selectedItemIds ?? new List<string>(),
            };

            return result;
        }

        private void AddChildNodes(Item parentItem, TreeNodeDto parentNode)
        {
            foreach (Item child in parentItem.Children)
            {
                var childNode = new TreeNodeDto
                {
                    Id = child.ID.ToString(),
                    Name = child.Name,
                    ShowCheckBox = true,
                    ImageUrl = child.Appearance.GetIconPath()
                };

                if (child.IsFolder() || child.Fields.All(f => f.Name.StartsWith("_"))) //todo: validation step
                {
                    childNode.ShowCheckBox = false;
                }

                parentNode.Children.Add(childNode);

                AddChildNodes(child, childNode);
            }
        }
    }
}