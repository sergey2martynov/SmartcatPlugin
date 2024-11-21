using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
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
        private readonly ISitecoreDbService _sitecoreDbService;

        public AuthController(ISmartcatApiClient apiClient,
            IAuthService authService,
            ISitecoreDbService sitecoreDbService)
        {
            _apiClient = apiClient;
            _authService = authService;
            _sitecoreDbService = sitecoreDbService;
        }

        [Route("save-apikey")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveCredentials(ApiKeyDto dto)
        {
            var isValid = await _apiClient.ValidateApiKeyAsync(dto);

            var apiKeyItem = _authService.GetApiKeyItem();

            var itemFields = new List<FieldDto>
            {
                new FieldDto
                {
                    Name = StringConstants.ApiKey,
                    Value = dto.ApiKey
                },
                new FieldDto
                {
                    Name = StringConstants.WorkSpaceId,
                    Value = dto.WorkspaceId
                }
            };

            _sitecoreDbService.UpdateFieldValues(apiKeyItem, itemFields);

            if (!isValid)
            {
                return BadRequest("Authorization was failed");
            }

            return Ok(true);
        }

        [Route("get-apikey")]
        [HttpGet]
        public IHttpActionResult GetCredentials()
        {
            var apiKeyItem = _authService.GetApiKeyItem();

            var result = new ApiKeyDto
            {
                WorkspaceId = apiKeyItem.Fields[StringConstants.WorkSpaceId].Value,
                ApiKey = apiKeyItem.Fields[StringConstants.ApiKey].Value
            };

            return Ok(result);
        }
    }
}