using System.Threading.Tasks;
using System.Web.Http;
using Sitecore.Data;
using Sitecore.SecurityModel;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly ISmartcatApiClient _apiClient;
        private readonly IAuthService _authService;
        private readonly Database _masterDb = Database.GetDatabase("master");

        public AuthController(ISmartcatApiClient apiClient,
            IAuthService authService)
        {
            _apiClient = apiClient;
            _authService = authService;
        }

        [Route("save-apikey")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveCredentials(ApiKeyDto dto)
        {
            var result = await _apiClient.ValidateApiKeyAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest("Authorization was failed");
            }

            var apiKeyItem = _authService.GetApiKeyItem(_masterDb);

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