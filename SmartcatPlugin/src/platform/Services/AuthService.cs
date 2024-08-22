using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.SecurityModel;
using System;
using Sitecore;
using SmartcatPlugin.Constants;

namespace SmartcatPlugin.Services
{
    public class AuthService
    {
        public void CreateApiKeyTemplate(Database database)
        {
            var templatesRoot = database.GetItem("/sitecore/templates/Smartcat");

            if (templatesRoot == null)
            {
                var templatesDirectory = database.GetItem("/sitecore/templates");
                templatesDirectory.Add("Smartcat", new TemplateID(ConstantIds.FolderTemplate));
                templatesRoot = database.GetItem("/sitecore/templates/Smartcat");
            }

            using (new SecurityDisabler())
            {

                TemplateItem baseTemplate = database.GetItem(TemplateIDs.Template);
                if (baseTemplate == null)
                {
                    throw new InvalidOperationException("Base template not found.");
                }

                var newTemplate = templatesRoot.Add("SmartcatApiKeyTemplate", baseTemplate);

                if (newTemplate == null)
                {
                    throw new InvalidOperationException("Template creation failed.");
                }

                newTemplate.Editing.BeginEdit();
                try
                {
                    TemplateSectionItem section = newTemplate.Add("Data", new TemplateID(TemplateIDs.TemplateSection));
                    if (section == null)
                    {
                        throw new InvalidOperationException("Failed to create 'Data' section.");
                    }

                    var textField1 = section.AddField("WorkSpaceId");
                    if (textField1 != null)
                    {
                        textField1.InnerItem.Editing.BeginEdit();
                        try
                        {
                            textField1.Type = "Single-Line Text";
                            textField1.Title = "WorkSpaceId";
                        }
                        finally
                        {
                            textField1.InnerItem.Editing.EndEdit();
                        }
                    }

                    var textField2 = section.AddField("ApiKey");
                    if (textField2 != null)
                    {
                        textField2.InnerItem.Editing.BeginEdit();
                        try
                        {
                            textField2.Type = "Single-Line Text";
                            textField2.Title = "ApiKey";
                        }
                        finally
                        {
                            textField2.InnerItem.Editing.EndEdit();
                        }
                    }
                }
                finally
                {
                    newTemplate.Editing.EndEdit();
                }
            }
        }

        public void CreateApiKeyItem(Database database)
        {
            TemplateItem templateItem = database.GetItem("/sitecore/templates/Smartcat/SmartcatApiKeyTemplate");

            var rootDirectory = database.GetItem("/sitecore/system/Settings");

            rootDirectory.Add("SmartcatApiKey", templateItem, ConstantIds.ApiKeyItem);
        }
    }
}