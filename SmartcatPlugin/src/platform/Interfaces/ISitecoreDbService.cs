using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Security.Accounts;
using SmartcatPlugin.Models.Dtos;
using System.Collections.Generic;

namespace SmartcatPlugin.Interfaces
{
    public interface ISitecoreDbService
    {
        Item GetItemById(ID id);
        Item GetItemByPath(string path);
        Item GetItemByIdAndLanguage(ID id, Language language);
        User GetCurrentUser();
        void UpdateFieldValues(Item item, List<FieldDto> fieldDtos);
    }
}
