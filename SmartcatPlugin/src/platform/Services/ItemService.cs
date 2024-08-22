﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.Dtos;
using HtmlAgilityPack;
using Sitecore.SecurityModel;
using SmartcatPlugin.Models.Smartcat.Testing;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands.IDE;

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

        public void CreateContentItem(TestDirectory rootDirectory)
        {
            var folderTemplateItem = _masterDb.GetItem(ConstantIds.FolderTemplate);
            Item newDirectoryItem;

            using (new SecurityDisabler())
            {
                var contentDirectory = _masterDb.GetItem(ConstantIds.ContentDirectory);

                newDirectoryItem = contentDirectory.Add(rootDirectory.Title, new TemplateID(folderTemplateItem.ID));
            }

            CreateChildrenDirectory(rootDirectory, newDirectoryItem);

            foreach (var page in rootDirectory.Pages)
            {
                var pageItem = CreatePage(newDirectoryItem, page);
                CreateChildrenPages(page, pageItem);
            }
        }

        private void CreateChildrenDirectory(TestDirectory parentDirectory, Item parentDirectoryItem)
        {
            var templateItem = _masterDb.GetItem(ConstantIds.FolderTemplate);
            foreach (var child in parentDirectory.Children)
            {
                Item newItem;
                using (new SecurityDisabler())
                {
                    newItem = parentDirectoryItem.Add(child.Title, new TemplateID(templateItem.ID));
                }
                CreateChildrenDirectory(child, newItem);
            }

            foreach (var page in parentDirectory.Pages)
            {
                var pageItem = CreatePage(parentDirectoryItem, page);
                CreateChildrenPages(page, pageItem);
            }
        }

        private void CreateChildrenPages(TestPage parentPage, Item parentPageItem)
        {
            foreach (var child in parentPage.Children)
            {
                var pageItem = CreatePage(parentPageItem, child);
                CreateChildrenPages(child, pageItem);
            }
        }

        private Item CreatePage(Item parentItem, TestPage page)
        {
            var pageTemplateItem = _masterDb.GetItem(ConstantIds.SampleItem);
            Language ruLanguage = Language.Parse("ru");
            Language esLanguage = Language.Parse("es");

            Item childItem;
            using (new SecurityDisabler())
            {
                var newItem = parentItem.Add(page.Title.EnglishValue, new TemplateID(pageTemplateItem.ID));
                childItem = newItem;
                using (new EditContext(newItem))
                {
                    newItem.Editing.BeginEdit();
                    newItem.Fields["Title"].Value = page.Title.EnglishValue;
                    newItem.Fields["Text"].Value = page.Content.EnglishValue;
                    newItem.Editing.EndEdit();
                }

                if (!string.IsNullOrEmpty(page.Title.RussianValue))
                {
                    newItem = _masterDb.GetItem(newItem.ID, ruLanguage);

                    newItem.Editing.BeginEdit();
                    newItem.Fields["Title"].Value = page.Title.RussianValue;
                    newItem.Fields["Text"].Value = page.Content.RussianValue;
                    newItem.Editing.EndEdit();
                }
                if (!string.IsNullOrEmpty(page.Title.SpanishValue))
                {
                    newItem = _masterDb.GetItem(newItem.ID, esLanguage);

                    newItem.Editing.BeginEdit();
                    newItem.Fields["Title"].Value = page.Title.SpanishValue;
                    newItem.Fields["Text"].Value = page.Content.SpanishValue;
                    newItem.Editing.EndEdit();
                }
            }

            return childItem;
        }
    }
}