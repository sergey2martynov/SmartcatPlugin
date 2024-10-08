using System;
using Newtonsoft.Json;
using Sitecore.Data;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Constants;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Data.Items;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models;
using Sitecore.Globalization;
using SmartcatPlugin.Interfaces;
using Sitecore.Data.Validators;

namespace SmartcatPlugin.Services
{
    public class BasketService : IBasketService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");
        private TreeNodeDto _rootNode;

        public ItemsTreeDto BuildSelectedItemTree()
        {
            string cachedData = CustomCacheManager.GetCache("selectedItems");
            if (string.IsNullOrEmpty(cachedData))
            {
                return new ItemsTreeDto(); // todo: exception
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
                else if (itemIds.Contains(item.ID.ToString()))
                {
                    var addedItem = addedItems[item.ID.ToString()];
                    addedItem.IsChecked = true;
                    addedItem.ShowCheckBox = true;
                }
            }

            var result = new ItemsTreeDto
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
                ShowCheckBox = true,
                ImageUrl = item.Appearance.GetIconPath(),
                IsChecked = true,
                IsExpanded = true
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
                        ImageUrl = item.Parent.Appearance.GetIconPath(),
                        IsExpanded = true
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
                    Id = item.ParentID.ToString(),
                    Name = parentItem.Name,
                    ImageUrl = item.Parent.Appearance.GetIconPath(),
                    IsExpanded = true
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

        public List<Item> GetItemsByIds(Database database, List<string> ids, string language)
        {
            List<Item> items = new List<Item>();
            Language itemLanguage = Language.Parse(language);

            foreach (var id in ids)
            {
                ID itemId = new ID(id);
                Item item = database.GetItem(itemId, itemLanguage);

                if (item == null || item.Versions.Count == 0)
                {
                    throw new InvalidOperationException($"Item with ID {id} does not exist or " +
                                                        $"does not have a version in language {language}.");
                }

                items.Add(item);
            }

            return items;
        }

        public LanguageDto GetDefaultLanguage()
        {
            var defaultLanguageCode = Sitecore.Context.Site?.Language == null ? "en" : Sitecore.Context.Site?.Language;
            var defaultLanguage = Language.Parse(defaultLanguageCode);

            var languageDto = new LanguageDto
            {
                Name = defaultLanguage.CultureInfo.EnglishName,
                Code = defaultLanguage.Name
            };

            return languageDto;
        }

        public List<LanguageDto> GetAvailableLanguages()
        {
            var languageItems = _masterDb.GetItem("/sitecore/system/Languages");
            var languageDtos = new List<LanguageDto>();

            if (languageItems != null)
            {
                foreach (Item languageItem in languageItems.Children)
                {
                    var language = Language.Parse(languageItem.Name);

                    var languageDto = new LanguageDto
                    {
                        Name = language.CultureInfo.EnglishName,
                        Code = language.Name
                    };
                    languageDtos.Add(languageDto);
                }
            }

            return languageDtos;
        }
    }
}