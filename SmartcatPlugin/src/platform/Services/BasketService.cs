using Newtonsoft.Json;
using Sitecore.Data;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Constants;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models;

namespace SmartcatPlugin.Services
{
    public class BasketService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");
        private TreeNodeDto _rootNode;

        public SelectedItemsDto BuildSelectedItemTree()
        {
            string cachedData = CustomCacheManager.GetCache("selectedItems");
            if (string.IsNullOrEmpty(cachedData))
            {
                return new SelectedItemsDto(); // todo: exception
            }

            var itemIds = JsonConvert.DeserializeObject<List<string>>(cachedData);
            var addedItems = new Dictionary<string, TreeNodeDto>();

            foreach (var itemId in itemIds)
            {
                var item = _masterDb.GetItem(new ID(itemId));
                if (item != null && !addedItems.ContainsKey(item.ID.ToString()))
                {
                    AddNodeWithParents(item, addedItems);
                }
            }

            var result = new SelectedItemsDto
            {
                TreeNodes = new List<TreeNodeDto> { _rootNode },
                CheckedItems = itemIds,
                ExpandedItems = addedItems.Keys.ToList(),
            };

            return result;
        }

        private void AddNodeWithParents(Item item, Dictionary<string, TreeNodeDto> addedItems)
        {
            var node = new TreeNodeDto
            {
                Id = item.ID.ToString(),
                Name = item.DisplayName,
                ShowCheckbox = true,
                ImageUrl = item.Appearance.GetIconPath()
            };

            addedItems.Add(item.ID.ToString(), node);

            AddParentNodes(item, addedItems, node);
        }

        private void AddParentNodes(Item item, Dictionary<string, TreeNodeDto> addedItems, TreeNodeDto childNode)
        {
            if (item.ParentID == ConstantIds.ContentDirectory)
            {
                TreeNodeDto rootNode;
                if (_rootNode == null)
                {
                    rootNode = new TreeNodeDto
                    {
                        Id = item.ID.ToString(),
                        Name = item.Parent.Name,
                        ImageUrl = item.Appearance.GetIconPath()
                    };
                    _rootNode = rootNode;
                    addedItems.Add(item.ParentID.ToString(), rootNode);
                }

                rootNode = _rootNode;
                if (!rootNode.Children.Contains(childNode))
                {
                    rootNode.Children.Add(childNode);
                }

                return;
            }

            var parentItem = item.Parent;

            TreeNodeDto parentNode;
            if (!addedItems.ContainsKey(parentItem.ID.ToString()))
            {
                parentNode = new TreeNodeDto
                {
                    Id = item.ID.ToString(),
                    Name = parentItem.Name,
                    ImageUrl = item.Appearance.GetIconPath()
                };
                addedItems.Add(item.ParentID.ToString(), parentNode);
                parentNode.Children.Add(childNode);
            }
            else
            {
                parentNode = addedItems[parentItem.ID.ToString()];
                parentNode.Children.Add(childNode);
                return;
            }

            AddParentNodes(parentItem, addedItems, parentNode);
        }
    }
}