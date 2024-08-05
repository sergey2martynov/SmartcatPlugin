using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.Dtos;
using HtmlAgilityPack;

namespace SmartcatPlugin.Services
{
    public class ItemService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        public ItemsTreeDto GetContentEditorItemsTree()
        {
            var rootItem = _masterDb.GetItem("/sitecore/content");
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

        public List<string> GetInvalidItemsNames(List<string> itemIds)
        {
            var invalidNames = new List<string>();

            foreach (var itemId in itemIds)
            {
                var id = new ID(itemId);
                var item = _masterDb.GetItem(id);

                if (item.Locking.IsLocked())
                {
                    invalidNames.Add(item.Name);
                    continue;
                }

                var fields = item.GetNonSystemFields();

                foreach (var field in fields)
                {
                    if (field.Type != ConstantItemFieldTypes.RichText)
                    {
                        continue;
                    }

                    var doc = new HtmlDocument();
                    doc.OptionFixNestedTags = false;

                    doc.LoadHtml(field.Value);

                    foreach (var error in doc.ParseErrors)
                    {
                        if (!invalidNames.Contains(item.Name))
                        {
                            invalidNames.Add(item.Name);
                        }
                    }
                }
            }

            return invalidNames;
        }
    }
}