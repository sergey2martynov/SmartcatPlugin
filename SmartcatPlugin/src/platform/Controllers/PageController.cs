using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Services.Core;
using SmartcatPlugin.Models;

namespace SmartcatPlugin.Controllers
{
    [ServicesController]
    public class PageController : ApiController
    {
        private readonly Database _masterDb;

        public PageController()
        {
            _masterDb = Database.GetDatabase("master");
        }

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
    }
}