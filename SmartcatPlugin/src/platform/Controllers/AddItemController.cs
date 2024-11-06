using System.Web.Http;
using Newtonsoft.Json;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Interfaces;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/additem")]
    public class AddItemController : ApiController
    {
        private readonly ICacheService _cacheService;
        private readonly IItemService _itemService;
        private readonly ISitecoreDbService _sitecoreDbService;
        private readonly ISmartcatLoggingService _logger;

        public AddItemController(ICacheService cacheService,
            IItemService itemService,
            ISmartcatLoggingService logger,
            ISitecoreDbService sitecoreDbService)
        {
            _cacheService = cacheService;
            _itemService = itemService;
            _sitecoreDbService = sitecoreDbService;
            _logger = logger;
        }

        [Route("get-items-tree")]
        [HttpGet]
        public IHttpActionResult GetItemsTree()
        {
            var result = _itemService.GetContentEditorItemsTree();

            return Json(result);
        }

        [Route("add-items")]
        [HttpPost]
        public IHttpActionResult SaveItemIdsToCache([FromBody] SaveItemIdsToCacheDto selectedItemIds)
        {
            var userName = _sitecoreDbService.GetCurrentUser().Name;
            var serializedList = JsonConvert.SerializeObject(selectedItemIds.SelectedItemIds);
            var cacheKey = $"{userName}:{StringConstants.SelectedItems}";
            _cacheService.SetValue(cacheKey, serializedList);
            _logger.LogInfo($"Item ids added to cache: {cacheKey}");
            return Ok();
        }
    }
}