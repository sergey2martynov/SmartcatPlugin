using Sitecore.Data.Items;
using Sitecore.Data;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Interfaces
{
    public interface IAuthService
    {
        TemplateItem CreateApiKeyTemplate(Database database);
        Item GetApiKeyItem(Database database);
        ApiKeyDto GetApiKey();
        string GetWorkspaceId();
    }
}
