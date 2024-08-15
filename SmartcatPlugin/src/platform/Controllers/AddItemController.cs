using SmartcatPlugin.Services;
using System.Web.Http;
using Newtonsoft.Json;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/additem")]
    public class AddItemController : ApiController
    {
        [Route("get-items-tree")]
        [HttpGet]
        public IHttpActionResult GetItemsTree()
        {
            var itemService = new ItemService();
            var result = itemService.GetContentEditorItemsTree();

            return Json(result);
        }

        [Route("add-items")]
        [HttpPost]
        public IHttpActionResult SaveItemIdsToCache([FromBody] SaveItemIdsToCacheDto selectedItemIds)
        {
            var serializedList = JsonConvert.SerializeObject(selectedItemIds.SelectedItemIds);
            CustomCacheManager.SetCache("selectedItems", serializedList);

            return Ok();
        }
    }
}