using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Services
{
    public class SitecoreDbService : ISitecoreDbService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        public Item GetItemById(ID id)
        {
            var item = _masterDb.GetItem(id);
            return item;
        }

        public Item GetItemByPath(string path)
        {
            var item = _masterDb.GetItem(path);
            return item;
        }

        public Item GetItemByIdAndLanguage(ID id, Language language)
        {
            var item = _masterDb.GetItem(id, language);
            return item;
        }

        public void UpdateFieldValues(Item item, List<FieldDto> fieldDtos)
        {
            using (new SecurityDisabler())
            {
                try
                {
                    item.Editing.BeginEdit();

                    foreach (var field in fieldDtos)
                    {
                        item.Fields[field.Name].Value = field.Value;
                    }
                }
                finally
                {
                    item.Editing.EndEdit();
                }
            }
        }

        public User GetCurrentUser()
        {
            var user = Sitecore.Context.User;
            return user;
        }
    }
}