using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch;
using SmartcatPlugin.Models.Smartcat.GetFileList;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.Smartcat;

namespace SmartcatPlugin.Services
{
    public class ItemLocalizationService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        public List<DataItem> GetChildItemsWithLocalizations(ID parentItemId, string searchQuery)
        {
            using (var context = ContentSearchManager.GetIndex($"sitecore_{_masterDb.Name}_index").CreateSearchContext())
            {
                var childItems = context.GetQueryable<SearchResultItem>()
                    .Where(item => item.Parent == parentItemId)
                    .WhereNameContains(searchQuery)
                    .Select(item => new
                    {
                        item.ItemId,
                        item.Name,
                        item.Language,
                        item.Fields,
                        item.Version
                    })
                    .ToList();

                var childPages = childItems.Where(item => item.Fields.ContainsKey(Sitecore.FieldIDs.LayoutField.ToString()) &&
                                                          item.Fields[Sitecore.FieldIDs.LayoutField.ToString()] != null).ToList();


                var childPageGgoups = childPages
                    .GroupBy(item => new { item.ItemId, item.Language })
                    .Select(group => group.OrderByDescending(item => item.Version).FirstOrDefault())
                    .ToList();

                    /*.Where(item => item.Fields.ContainsKey(Sitecore.FieldIDs.LayoutField.ToString()) &&
                                   item.Fields[Sitecore.FieldIDs.LayoutField.ToString()] != null)
                    .GroupBy(item => item.ItemId)
                    .Select(group => new
                    {
                        ItemId = group.Key,
                        Items = group.ToList()
                    })
                    .ToList();*/

                var dataItems = new List<DataItem>();

                /*foreach (var childPage in childPageGroups)
                {
                    var localizedItemInfo = new DataItem()
                    {
                        Id = new ExternalObjectId { ExternalId = childPage.ItemId.ToString(), ExternalType = ConstantItemTypes.Page },
                        ParentDirectoryIds = new List<ExternalObjectId>
                        {
                            new ExternalObjectId{ ExternalId = parentItemId.ToString(), ExternalType = ConstantItemTypes.Folder }
                        },
                        Name = childPage.Items[0].Name,
                        Locales = childPage.Items.Select(item => item.Language).ToList()
                    };

                    dataItems.Add(localizedItemInfo);
                }*/

                return dataItems;
            }
        }
    }
}