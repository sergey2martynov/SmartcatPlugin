using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.Smartcat.GetDirectoryList;
using SmartcatPlugin.Models.Smartcat.GetFileContent;
using SmartcatPlugin.Models.Smartcat.GetFileList;
using SmartcatPlugin.Tools;

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
                if (childItem.IsFolder())            //todo make base template finder
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

            //todo one query to db
            /*var allChildren = parentItem.Axes.GetDescendants()
                .Where(item => string.IsNullOrEmpty(request.SearchQuery) || item.Name.Contains(request.SearchQuery))
                .ToList()*/

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

        public static Dictionary<string, LocJsonContent> GetItemContent(this Item item, Database masterDb, FileContentRequest request)
        {
            if (item == null || masterDb == null || request == null)
            {
                return new Dictionary<string, LocJsonContent>();
            }

            var parentItemLanguage = item.Language;
            var allChildren = item.Axes.GetDescendants()
                .Where(childItem => childItem.Language.Equals(parentItemLanguage) 
                                    && (childItem.IsHaveLayout() || !childItem.IsFolder()))
                .GroupBy(childItem => childItem.ID)
                .Select(group => group.OrderByDescending(childItem => childItem.Version.Number).First())
                .ToList();

            var targetLanguages = request.TargetLocales
                .Select(Language.Parse)
                .ToList();

            var locJsonDictionary = new Dictionary<string, LocJsonContent>();

            var units = new List<Unit>();

            foreach (var children in allChildren)
            {
                var fields = item.Fields
                    .Where(f => !f.Name.StartsWith("_") && f.HasValue);

                foreach (var field in fields)
                {
                    var unit = new Unit
                    {
                        Key = children.Paths.Path + "/" + field.Key,
                        Properties = new UnitProperties(),
                        Source = StringSplitter.SplitStringWithNewlines(field.Value)
                    };

                    units.Add(unit);
                }
            }

            var locJsonContent = new LocJsonContent { Units = units };

            foreach (var targetLanguage in targetLanguages)
            {
                locJsonDictionary.Add(targetLanguage.Name, locJsonContent);
            }

            return locJsonDictionary;
        }

        private static bool IsFolder(this Item item)
        {
            if (item.TemplateID == ConstantIds.FolderTemplate
                || !item.IsHaveLayout() && item.HasChildren)
            {
                return true;
            }

            return false;
        }
    }
}