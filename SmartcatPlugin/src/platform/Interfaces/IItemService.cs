using SmartcatPlugin.Models;
using SmartcatPlugin.Models.Smartcat.Testing;
using System.Collections.Generic;
using Sitecore.Data.Items;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Interfaces
{
    public interface IItemService
    {
        AddedItemsTreeDto GetContentEditorItemsTree();
        //List<string> GetInvalidItemsNames(List<string> itemIds);
        void CreateContentItem(TestDirectory rootDirectory);
        Dictionary<string, LocJsonContent> GetItemContent(Item parentPage, List<string> targetLocales);
    }
}
