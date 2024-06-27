using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Services.Core;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Models;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.Smartcat.GetDirectoryList;

namespace SmartcatPlugin.Controllers
{
    [ServicesController]
    public class PageController : ApiController
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        [HttpGet]
        public HttpResponseMessage Index()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

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

        [HttpPost]
        [Route("directory-list")]
        public HttpResponseMessage GetDirectoryList([FromBody] GetDataDirectoriesRequest request)
        {
            Item rootItem;

            if (request.ParentDirectoryId.ExternalId == "root")
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
                var badResponse = new HttpResponseMessage(HttpStatusCode.NotFound) //todo create method
                {
                    Content = new StringContent("Directory not found"),
                    ReasonPhrase = "Directory Not Found"
                };

                return badResponse;
            }

            var childrenItems = rootItem.Children.ToList();

            var getDataDirectoriesResponse = new GetDataDirectoriesResponse
            {
                NextBatchKey = null,
                Directories = GetChildDirectories(childrenItems)
            };

            var json = JsonConvert.SerializeObject(getDataDirectoriesResponse);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return response;
        }

        private List<DataDirectory> GetChildDirectories(List<Item> childList)
        {
            if (childList == null || !childList.Any())
            {
                return new List<DataDirectory>();
            }

            var directories = new List<DataDirectory>();

            foreach (var item in childList)
            {
                if (item.TemplateID == ConstantIds.FolderTemplateID) //todo make base template finder
                {
                    directories.Add(new DataDirectory
                    {
                        Id = new ExternalObjectId { ExternalId = item.ID.ToString(), ExternalType = "Folder" },
                        Name = item.Name,
                        CanLoadChildDirectories = true,
                        CanLoadChildItems = true,
                        ChildDirectories = GetChildDirectories(item.Children.ToList())
                    });
                }
            }

            return directories;
        }
    }
}