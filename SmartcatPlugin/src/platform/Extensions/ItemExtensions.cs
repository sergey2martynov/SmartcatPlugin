using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands.TemplateBuilder;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Models;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.Smartcat.GetFolderList;
using SmartcatPlugin.Models.Smartcat.GetItemContent;
using SmartcatPlugin.Models.Smartcat.GetItemList;
using SmartcatPlugin.Tools;

namespace SmartcatPlugin.Extensions
{
    public static class ItemExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(LogNames.SmartcatApi);

        public static List<DataDirectory> GetChildDirectories(this Item item)
        {
            var childList = item.Children.ToList();

            if (!childList.Any())
            {
                return new List<DataDirectory>();
            }

            var directories = new List<DataDirectory>();

            foreach (var childItem in childList)
            {
                if (childItem.HasChildren)
                {
                    directories.Add(new DataDirectory
                    {
                        Id = new ExternalObjectId { ExternalId = childItem.ID.ToString(), ExternalType = StringConstants.Directory },
                        Name = childItem.Name,
                        CanLoadChildDirectories = true,
                        CanLoadChildItems = true,
                        ChildDirectories = GetChildDirectories(childItem)
                    });

                    Log.Info($"DataDirectory ${childItem.Name} with Id {childItem.ID} was created. ItemExtensions.GetChildPages()");
                }
            }

            return directories;
        }

        public static bool IsHasContentFields(this Item item)
        {
            if (item.Fields.All(f => f.Name.StartsWith("_")))
            {
                return false;
            }

            return true;
        }

        public static List<DataItem> GetChildPages(this Item parentItem, string searchQuery, Database masterDb)
        {
            var childList = parentItem.Children.ToList();

            if (!childList.Any())
            {
                Log.Info($"Item ${parentItem.Name} with Id {parentItem.ID} do not contain children. ItemExtensions.GetChildPages()");
                return new List<DataItem>();
            }

            var pages = new List<DataItem>();

            foreach (var childItem in childList)
            {
                var matchesSearchQuery = !string.IsNullOrEmpty(searchQuery) && childItem.Name.Contains(searchQuery);
                var isSearchQueryEmpty = string.IsNullOrEmpty(searchQuery);

                if (childItem.IsHasContentFields() && (matchesSearchQuery || isSearchQueryEmpty))
                {
                    pages.Add(new DataItem
                    {
                        Id = new ExternalObjectId { ExternalId = childItem.ID.ToString(), ExternalType = StringConstants.Item },
                        ParentDirectoryIds = new List<ExternalObjectId>
                        {
                            new ExternalObjectId{ ExternalId = parentItem.ID == ConstantIds.ContentDirectory ? ConstantIds.Root : parentItem.ID.ToString(), ExternalType = StringConstants.Directory }
                        },
                        Name = childItem.Name,
                        Locales = childItem.GetItemLocales(masterDb)
                    });

                    Log.Info($"DataItem ${childItem.Name} with Id {childItem.ID} was created. ItemExtensions.GetChildPages()");
                }
            }

            return pages;
        }

        public static List<string> GetItemLocales(this Item item, Database masterDb)
        {
            if (item == null)
            {
                Log.Warn($"Item is null. ItemExtensions.GetItemLocales()");
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

        public static Dictionary<string, LocJsonContent> GetItemContent(this Item parentPage, Database masterDb,
            string[] targetLocales)
        {
            if (parentPage == null || masterDb == null || targetLocales == null)
            {
                throw new NullReferenceException("Invalid inner data");
            }

            var targetLanguages = targetLocales
                .Select(Language.Parse)
                .ToList();

            var locJsonDictionary = new Dictionary<string, LocJsonContent>();

            var units = new List<Unit>();

            var fields = parentPage.GetNonSystemFields();

            foreach (var field in fields)
            {
                var unit = new Unit
                {
                    Key = field.Key,
                    Properties = new UnitProperties
                    {
                        SmartcatFormat = field.Type == ConstantItemFieldTypes.RichText ? "html" : ""
                    },
                    Source = StringSplitter.SplitStringWithNewlines(field.Value),
                    Target = new List<string>()
                };
                
                units.Add(unit);
                Log.Info($"{typeof(Unit)} key:{unit.Key} was created. ItemExtensions.GetItemContent()");
            }

            foreach (var targetLanguage in targetLanguages)
            {
                var locJsonContent = new LocJsonContent
                {
                    Units = units,
                    Properties = new Properties
                    {
                        ItemId = parentPage.ID.ToString(),
                        TargetLanguage = targetLanguage.Name
                    }
                };

                locJsonDictionary.Add(targetLanguage.Name, locJsonContent);

                var versions = parentPage.Versions.GetVersions(true);
                var isItemHaveTargetLanguages = versions.Any(v => v.Language == targetLanguage);

                if (!isItemHaveTargetLanguages)
                {
                    continue;
                }

                var targetVersionItem = masterDb.GetItem(parentPage.ID, targetLanguage);

                foreach (var unit in locJsonContent.Units)
                {
                    var field = targetVersionItem.Fields[unit.Key];
                    unit.Target = StringSplitter.SplitStringWithNewlines(field.Value);
                }
            }

            return locJsonDictionary;
        }

        public static List<Item> GetAllChildrenPages(this Item item)
        {
            var allChildren = item.Axes.GetDescendants()
                .Where(childItem => childItem.Language.Equals(item.Language)
                                    && (childItem.IsHasContentFields() || !childItem.IsFolder()))
                .GroupBy(childItem => childItem.ID)
                .Select(group => group.OrderByDescending(childItem => childItem.Version.Number).First())
                .ToList();

            return allChildren;
        }

        public static string GetPathWithIds(this Item childrenPage, Item parentPage)
        {
            if (childrenPage == null)
            {
                return string.Empty;
            }

            var pathWithIds = new StringBuilder();
            var currentItem = childrenPage;

            if (childrenPage == parentPage)
            {
                pathWithIds.Insert(0, $"/{currentItem.ID}");
                return pathWithIds.ToString();
            }

            while (currentItem != null && currentItem.ID != parentPage.ID)
            {
                pathWithIds.Insert(0, $"/{currentItem.ID}");
                currentItem = currentItem.Parent;
            }

            pathWithIds.Insert(0, $"{parentPage.ID}");

            return pathWithIds.ToString();
        }

        public static bool IsFolder(this Item item)
        {
            if (item.TemplateID == ConstantIds.FolderTemplate
                || !item.IsHasContentFields() && item.HasChildren)
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<Field> GetNonSystemFields(this Item item)
        {
            var fields = item.Fields
                .Where(f => !f.Name.StartsWith("_") && f.Type.IsTranslatedType() && f.HasValue);

            return fields;
        }

        public static List<Field> GetRichTextField(this Item item)
        {
            var result = item.Fields
                .Where(f => f.Type == ConstantItemFieldTypes.RichText && !f.Name.StartsWith("_")).ToList();
            return result;
        }
    }
}