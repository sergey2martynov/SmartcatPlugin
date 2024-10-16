﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.Dtos;
using Sitecore.SecurityModel;
using SmartcatPlugin.Models.Smartcat.Testing;
using Sitecore.Globalization;
using Sitecore.Data.Validators;
using Sitecore.Data.Validators.FieldValidators;
using SmartcatPlugin.Interfaces;

namespace SmartcatPlugin.Services
{
    public class ItemService : IItemService
    {
        private readonly ICacheService _cacheService;
        private readonly Database _masterDb = Database.GetDatabase("master");
        private readonly Database _webDb = Database.GetDatabase("web");

        public ItemService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public AddedItemsTreeDto GetContentEditorItemsTree()
        {
            var rootItem = _masterDb.GetItem("/sitecore/content");
            var userName = Sitecore.Context.User.Name;
            string cachedData = _cacheService.GetValue($"{userName}:{StringConstants.SelectedItems}");
            var selectedItemIds = cachedData == null ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(cachedData);

            var selectedNodes = new Dictionary<string, TreeNodeDto>();

            foreach (var id in selectedItemIds)
            {
                selectedNodes[id] = null;
            }

            var rootNode = new TreeNodeDto
            {
                Id = rootItem.ID.ToString(),
                Name = rootItem.Name,
                ShowCheckBox = false,
                ImageUrl = rootItem.Appearance.GetIconPath(),
                IsChecked = selectedItemIds.Contains(rootItem.ID.ToString()),
                IsExpanded = true
            };

            AddChildNodes(rootItem, rootNode, selectedNodes);

            var result = new AddedItemsTreeDto
            {
                TreeNodes = new List<TreeNodeDto> { rootNode },
                CheckedItems = selectedNodes.Values.ToList(),
                ExpandedItems = selectedNodes.Values.ToList(),
            };

            return result;
        }

        private void AddChildNodes(Item parentItem, TreeNodeDto parentNode, Dictionary<string, TreeNodeDto> selectedNodes)
        {
            foreach (Item child in parentItem.Children)
            {
                var childId = child.ID.ToString();

                var isContain = selectedNodes.Keys.Contains(childId);

                var childNode = new TreeNodeDto
                {
                    Id = childId,
                    Name = child.Name,
                    ShowCheckBox = true,
                    ImageUrl = child.Appearance.GetIconPath(),
                    IsChecked = isContain,
                    IsExpanded = isContain
                };

                if (isContain)
                {
                    selectedNodes[childId] = childNode;
                }

                if (child.IsFolder() || child.Fields.All(f => f.Name.StartsWith("_"))) //todo: validation step
                {
                    childNode.ShowCheckBox = false;
                }

                parentNode.Children.Add(childNode);

                AddChildNodes(child, childNode, selectedNodes);

                if (childNode.IsExpanded)
                {
                    parentNode.IsExpanded = true;
                }
            }
        }

        public List<string> GetInvalidItemsNames(List<string> itemIds)
        {
            var invalidNames = new List<string>();
            Sitecore.Context.ContentDatabase = _masterDb;

            foreach (var itemId in itemIds)
            {
                var id = new ID(itemId);
                var item = _masterDb.GetItem(id);
                List<FieldDescriptor> fieldDescriptors = new List<FieldDescriptor>();

                if (item.Locking.IsLocked())
                {
                    invalidNames.Add(item.Name);
                    continue;
                }

                var notSystemFields = item.GetNonSystemFields();
                if (notSystemFields.Any(f => string.IsNullOrEmpty(f.Value)))
                {
                    invalidNames.Add(item.Name);
                    continue;
                }

                var richTextFields = item.GetRichTextField();
                if (richTextFields.Any())
                {
                    richTextFields.ForEach(f => fieldDescriptors.Add(new FieldDescriptor(item, f.Name)));
                }

                ValidatorCollection validatorsForTitle =
                    ValidatorManager.GetFieldsValidators(ValidatorsMode.ValidateButton, fieldDescriptors, _masterDb);

                foreach (BaseValidator validator in validatorsForTitle)
                {
                    validator.Validate(new ValidatorOptions(false));

                    if (validator is XhtmlValidator && !validator.IsValid)
                    {
                        invalidNames.Add(item.Name);
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
            if (parentDirectory.Children != null)
            {
                foreach (var child in parentDirectory.Children)
                {
                    Item newItem;
                    using (new SecurityDisabler())
                    {
                        newItem = parentDirectoryItem.Add(child.Title, new TemplateID(templateItem.ID));
                    }
                    CreateChildrenDirectory(child, newItem);
                }
            }

            if (parentDirectory.Pages != null)
            {
                foreach (var page in parentDirectory.Pages)
                {
                    var pageItem = CreatePage(parentDirectoryItem, page);
                    CreateChildrenPages(page, pageItem);
                }
            }
            
        }

        private void CreateChildrenPages(TestPage parentPage, Item parentPageItem)
        {
            if (parentPage.Children == null)
            {
                return;
            }

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