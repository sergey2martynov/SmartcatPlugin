using System;
using SmartcatPlugin.Services;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Security.Accounts;
using SmartcatPlugin.Cache;
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

        public AddItemController(ICacheService cacheService,
            IItemService itemService)
        {
            _cacheService = cacheService;
            _itemService = itemService;
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
            var userName = Sitecore.Context.User.Name;
            var serializedList = JsonConvert.SerializeObject(selectedItemIds.SelectedItemIds);
            _cacheService.SetValue($"{userName}:{StringConstants.SelectedItems}", serializedList);

            return Ok();
        }
    }
}