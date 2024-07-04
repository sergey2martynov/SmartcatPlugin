using System.Collections.Generic;
using System.Web.Http;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.Smartcat.GetDirectoryList;
using SmartcatPlugin.Models.Smartcat.GetFileContent;
using SmartcatPlugin.Models.Smartcat.GetFileList;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("smartcat")]
    public class PageController : ApiController
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        [Route("directory-list")]
        [HttpPost]
        public IHttpActionResult GetDirectoryList([FromBody] GetDataDirectoriesRequest request)
        {
            Item rootItem;

            if (request.ParentDirectoryId.ExternalId.ToLower() == ConstantIds.Root)
            {
                rootItem = _masterDb.GetItem("/sitecore/content/");
            }
            else
            {
                var id = new ID(request.ParentDirectoryId.ExternalId);
                rootItem = _masterDb.GetItem(id);
            }

            if (rootItem == null)
            {
                return NotFound();
            }

            var getDataDirectoriesResponse = new GetDataDirectoriesResponse
            {
                NextBatchKey = null,
                Directories = rootItem.GetChildDirectories()
            };

            return Json(getDataDirectoriesResponse);
        }

        [Route("file-list")]
        [HttpPost]
        public IHttpActionResult GetPageList([FromBody] GetDataItemsRequest request)
        {
            Item rootItem;

            if (request.ParentDirectoryId.ExternalType != ConstantItemTypes.Folder)
            {
                return Json(GetDataItemsResponse.Empty);
            }

            if (request.ParentDirectoryId.ExternalId.ToLower() == ConstantIds.Root)
            {
                rootItem = _masterDb.GetItem("/sitecore/content/");
            }
            else
            {
                var id = new ID(request.ParentDirectoryId.ExternalId);
                rootItem = _masterDb.GetItem(id);
            }

            if (rootItem == null)
            {
                return NotFound();
            }

            var getDataItemsResponse = new GetDataItemsResponse
            {
                NextBatchKey = null,
                Items = rootItem.GetChildPages(request.SearchQuery, _masterDb)
            };

            return Json(getDataItemsResponse);
        }

        [Route("file-content")]
        [HttpPost]
        public IHttpActionResult GetFileContent([FromBody] FileContentRequest request)
        {
            var id = new ID(request.ItemId.ExternalId);
            var item = _masterDb.GetItem(id, Language.Parse(request.SourceLocale));

            if (!item.IsHaveLayout())
            {
                return Json(new Dictionary<string, LocJsonContent>());
            }

            var result = item.GetItemContent(_masterDb, request);

            return Json(result);
        }
    }
}