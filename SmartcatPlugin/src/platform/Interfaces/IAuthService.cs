using Sitecore.Data.Items;
using Sitecore.Data;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Interfaces
{
    public interface IAuthService
    {
        TemplateItem CreateApiKeyTemplate();
        Item GetApiKeyItem();
        ApiKeyDto GetApiKey();
        string GetWorkspaceId();
    }
}
