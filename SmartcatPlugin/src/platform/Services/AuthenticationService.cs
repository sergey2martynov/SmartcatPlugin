using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace SmartcatPlugin.Services
{
    public class AuthenticationService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        public Item SaveToken(string smartcatAuthKey)
        {
            using (new SecurityDisabler())
            {
                var template = _masterDb.Templates["sitecore/Templates/SmartcatTokenTemplate"];

                if (template == null)
                {
                    template = CreateSmartcatTokenTemplate();
                }

                Item parentItem = _masterDb.GetItem("/sitecore/system");
                Item tokenItem = parentItem.Children["SmartcatToken"];

                if (tokenItem == null)
                {
                    tokenItem = parentItem.Add("SmartcatToken", new TemplateID(template.ID));
                }

                using (new EditContext(tokenItem))
                {
                    tokenItem["Key"] = smartcatAuthKey;
                }

                return tokenItem;
            }
        }

        private TemplateItem CreateSmartcatTokenTemplate()
        {
            var parentItem = _masterDb.GetItem("/sitecore/templates");
            var baseTemplate = _masterDb.GetTemplate(TemplateIDs.StandardTemplate);

            using (new EditContext(parentItem))
            {
                TemplateItem template = parentItem.Add("AuthenticationToken", new TemplateID(baseTemplate.ID));
                if (template != null)
                {
                    AddFieldToTemplate(template, "Key", "Single-Line Text");
                }
                return template;
            }
        }

        private void AddFieldToTemplate(TemplateItem template, string fieldName, string fieldType)
        {
            using (new SecurityDisabler())
            {
                using (new EditContext(template.InnerItem))
                {
                    TemplateSectionItem section = template.AddSection("Data");
                    TemplateFieldItem field = section.AddField(fieldName);
                    field.InnerItem["Type"] = fieldType;
                }
            }
        }
    }
}