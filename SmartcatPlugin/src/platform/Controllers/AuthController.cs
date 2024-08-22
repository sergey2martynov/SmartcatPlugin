using System.Threading.Tasks;
using System.Web.Http;
using Sitecore.Data;
using SmartcatPlugin.Models.Dtos;
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


            //_masterDb.GetItem()

            return Ok(result);
        }
    }
}