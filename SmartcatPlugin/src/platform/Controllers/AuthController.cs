using System.Threading.Tasks;
using System.Web.Http;
using Sitecore.Data;
using Sitecore.SecurityModel;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Services;
using SmartcatPlugin.Smartcat;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        [Route("save-apikey")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveCredentials(ApiKeyDto dto)
        {
            var client = new ApiClient();
            var result = await client.ValidateApiKeyAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest("Authorization was failed");
            }

            var authService = new AuthService();
            var apiKeyItem = authService.GetApiKeyItem(_masterDb);

            using (new SecurityDisabler())
            {
                try
                {
                    apiKeyItem.Editing.BeginEdit();
                    apiKeyItem.Fields[StringConstants.WorkSpaceId].Value = dto.WorkspaceId;
                    apiKeyItem.Fields[StringConstants.ApiKey].Value = dto.ApiKey;
                }
                finally
                {
                    apiKeyItem.Editing.EndEdit();
                }
            }

            return Ok(result);
        }
    }
}