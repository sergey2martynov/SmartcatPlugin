using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.SecurityModel;
using System;
using Sitecore;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Services
{
    public class AuthService : IAuthService
    {
        private readonly ISitecoreDbService _sitecoreDbService;

        public AuthService(ISitecoreDbService sitecoreDbService)
        {
            _sitecoreDbService = sitecoreDbService;
        }

        public TemplateItem CreateApiKeyTemplate()
        {
            var templatesRoot = _sitecoreDbService.GetItemByPath("/sitecore/templates/Smartcat");

            if (templatesRoot == null)
            {
                var templatesDirectory = _sitecoreDbService.GetItemByPath("/sitecore/templates");
                templatesDirectory.Add("Smartcat", new TemplateID(ConstantIds.FolderTemplate));
                templatesRoot = _sitecoreDbService.GetItemByPath("/sitecore/templates/Smartcat");
            }

            Item newTemplate;

            using (new SecurityDisabler())
            {
                TemplateItem baseTemplate = _sitecoreDbService.GetItemById(TemplateIDs.Template);
                if (baseTemplate == null)
                {
                    throw new InvalidOperationException("Base template not found.");
                }

                newTemplate = templatesRoot.Add("SmartcatApiKeyTemplate", baseTemplate);

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

                    var textField1 = section.AddField(StringConstants.WorkSpaceId);
                    if (textField1 != null)
                    {
                        textField1.InnerItem.Editing.BeginEdit();
                        try
                        {
                            textField1.Type = ConstantItemFieldTypes.SingleLineText;
                            textField1.Title = StringConstants.WorkSpaceId;
                        }
                        finally
                        {
                            textField1.InnerItem.Editing.EndEdit();
                        }
                    }

                    var textField2 = section.AddField(StringConstants.ApiKey);
                    if (textField2 != null)
                    {
                        textField2.InnerItem.Editing.BeginEdit();
                        try
                        {
                            textField2.Type = ConstantItemFieldTypes.SingleLineText;
                            textField2.Title = StringConstants.ApiKey;
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

            return newTemplate;
        }

        public Item GetApiKeyItem()
        {
            TemplateItem templateItem = _sitecoreDbService.GetItemByPath("/sitecore/templates/Smartcat/SmartcatApiKeyTemplate");

            if (templateItem == null)
            {
                templateItem = CreateApiKeyTemplate();
            }

            var apiKeyItem = _sitecoreDbService.GetItemById(ConstantIds.ApiKeyItem);

            if (apiKeyItem == null)
            {
                var settingsDirectory = _sitecoreDbService.GetItemByPath("/sitecore/system/Settings");
                var smartcatDirectory = _sitecoreDbService.GetItemByPath("/sitecore/system/Settings/Smartcat");

                if (smartcatDirectory == null)
                {
                    smartcatDirectory = settingsDirectory.Add("Smartcat", new TemplateID(ConstantIds.FolderTemplate), ID.NewID);
                }
                
                apiKeyItem = smartcatDirectory.Add(StringConstants.ApiKey, templateItem, ConstantIds.ApiKeyItem);
            }

            return apiKeyItem;
        }

        public ApiKeyDto GetApiKey()
        {
            var apiKeyItem = _sitecoreDbService.GetItemById(ConstantIds.ApiKeyItem);

            var apiKey = new ApiKeyDto
            {
                WorkspaceId = apiKeyItem.Fields[StringConstants.WorkSpaceId].Value,
                ApiKey = apiKeyItem.Fields[StringConstants.ApiKey].Value
            };

            return apiKey;
        }

        public string GetWorkspaceId()
        {
            var apiKeyItem = _sitecoreDbService.GetItemById(ConstantIds.ApiKeyItem);
            var workspaceId = apiKeyItem.Fields[StringConstants.WorkSpaceId].Value;

            return workspaceId;
        }
    }
}