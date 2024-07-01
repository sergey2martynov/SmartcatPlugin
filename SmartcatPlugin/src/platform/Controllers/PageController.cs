using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models;
using SmartcatPlugin.Models.Smartcat.GetDirectoryList;
using SmartcatPlugin.Models.Smartcat.GetFileList;
using SmartcatPlugin.Services;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("smartcat")]
    public class PageController : ApiController
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        [Route("home")]
        [HttpGet]
        public HttpResponseMessage GetHome()
        {
            Database database = Sitecore.Configuration.Factory.GetDatabase("master");
            Item rootItem = database.GetItem("/sitecore/content/home");

            if (rootItem == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var page = new PageModel()
            {
                Id = rootItem.ID,
                Name = rootItem.Name,
                Url = rootItem.Uri,
                Fields = rootItem.Fields.Select(f => new ItemField()
                {
                    Name = f.Name,
                    Value = f.Value
                })
            };

            var json = JsonConvert.SerializeObject(page);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return response;
        }

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

            var service = new ItemLocalizationService();

            var getDataItemsResponse = new GetDataItemsResponse
            {
                NextBatchKey = null,
                Items = rootItem.GetChildPages(request.SearchQuery, _masterDb)
            };

            return Json(getDataItemsResponse);
        }
    }
}