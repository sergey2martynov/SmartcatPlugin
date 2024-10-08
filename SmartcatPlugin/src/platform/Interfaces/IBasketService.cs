using Sitecore.Data;
using SmartcatPlugin.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace SmartcatPlugin.Interfaces
{
    public interface IBasketService
    {
        ItemsTreeDto BuildSelectedItemTree();
        List<Item> GetItemsByIds(Database database, List<string> ids, string language);
        LanguageDto GetDefaultLanguage();
        List<LanguageDto> GetAvailableLanguages();
    }
}
