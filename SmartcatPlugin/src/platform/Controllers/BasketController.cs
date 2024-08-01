using System.Threading;
using System.Web.Http;
using SmartcatPlugin.Services;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/basket")]
    public class BasketController : ApiController
    {
        [Route("get-selected-items")]
        [HttpGet]
        public IHttpActionResult GetSelectedItems()
        {
            var basketService = new BasketService();

            var result = basketService.BuildSelectedItemTree();

            return Json(result);
        }

        [Route("update-ribbon")]
        [HttpGet]
        public IHttpActionResult UpdateRibbon()
        {
            Thread.Sleep(5000);
            Sitecore.Context.ClientPage.ClientResponse.Timer("item:refresh", 2000);

            return Ok();
        }
    }
}