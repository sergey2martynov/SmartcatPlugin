using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.Smartcat.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartcatPlugin.Interfaces
{
    public interface IItemService
    {
        AddedItemsTreeDto GetContentEditorItemsTree();
        List<string> GetInvalidItemsNames(List<string> itemIds);
        void CreateContentItem(TestDirectory rootDirectory);
    }
}
