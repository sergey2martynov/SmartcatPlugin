using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;
using Newtonsoft.Json;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Models.Dtos;
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

        [Route("get-validating-info")]
        [HttpGet]
        public IHttpActionResult GetValidatingInfo()
        {
            string cachedData = CustomCacheManager.GetCache("selectedItems");
            if (string.IsNullOrEmpty(cachedData))
            {
                return Ok(new List<string>()); // todo: exception
            }

            var itemIds = JsonConvert.DeserializeObject<List<string>>(cachedData);
            var itemService = new ItemService();

            var invalidItemNames = itemService.GetInvalidItemsNames(itemIds);

            var validatingInfo = new ValidatingInfoDto
            {
                InvalidItemNames = string.Join(", ", invalidItemNames),
                InvalidItemCount = invalidItemNames.Count,
                ValidItemCount = itemIds.Count - invalidItemNames.Count
            };

            return Ok(validatingInfo);
        }
    }
}