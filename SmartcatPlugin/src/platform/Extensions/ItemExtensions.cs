using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.Smartcat.GetDirectoryList;
using SmartcatPlugin.Models.Smartcat.GetFileList;

namespace SmartcatPlugin.Extensions
{
    public static class ItemExtensions
    {
        public static List<DataDirectory> GetChildDirectories(this Item item)
        {
            var childList = item.Children.ToList();

            if (childList == null || !childList.Any())
            {
                return new List<DataDirectory>();
            }

            var directories = new List<DataDirectory>();

            foreach (var childItem in childList)
            {
                if (childItem.TemplateID == ConstantIds.FolderTemplate 
                    || !childItem.IsHaveLayout() && childItem.HasChildren)                  //todo make base template finder
                {
                    directories.Add(new DataDirectory
                    {
                        Id = new ExternalObjectId { ExternalId = childItem.ID.ToString(), ExternalType = "Folder" },
                        Name = item.Name,
                        CanLoadChildDirectories = true,
                        CanLoadChildItems = true,
                        ChildDirectories = GetChildDirectories(childItem)
                    });
                }
            }

            return directories;
        }

        public static bool IsHaveLayout(this Item item)
        {
            if (item == null)
            {
                return false;
            }

            var layoutField = item.Fields[Sitecore.FieldIDs.LayoutField];
            return layoutField != null && !string.IsNullOrEmpty(layoutField.Value);
        }

        public static List<DataItem> GetChildPages(this Item parentItem, string searchQuery, Database masterDb)
        {
            var childList = parentItem.Children.ToList();

            if (!childList.Any())
            {
                return new List<DataItem>();
            }

            var pages = new List<DataItem>();

            foreach (var childItem in childList)
            {
                var matchesSearchQuery = !string.IsNullOrEmpty(searchQuery) && childItem.Name.Contains(searchQuery);
                var isSearchQueryEmpty = string.IsNullOrEmpty(searchQuery);

                if (childItem.IsHaveLayout() && (matchesSearchQuery || isSearchQueryEmpty))
                {
                    pages.Add(new DataItem
                    {
                        Id = new ExternalObjectId { ExternalId = childItem.ID.ToString(), ExternalType = ConstantItemTypes.Page },
                        ParentDirectoryIds = new List<ExternalObjectId>
                        {
                            new ExternalObjectId{ ExternalId = parentItem.ID.ToString(), ExternalType = ConstantItemTypes.Folder }
                        },
                        Name = childItem.Name,
                        Locales = childItem.GetItemLocales(masterDb)
                    });
                }
            }

            return pages;
        }

        public static List<string> GetItemLocales(this Item item, Database masterDb)
        {
            if (item == null)
            {
                return new List<string>();
            }

            var locales = new List<string>();

            foreach (Language language in item.Languages)
            {
                Item versionedItem = masterDb.GetItem(item.ID, language);

                if (versionedItem != null && versionedItem.Versions.Count > 0)
                {
                    locales.Add(language.Name);
                }
            }

            return locales;
        }
    }
}