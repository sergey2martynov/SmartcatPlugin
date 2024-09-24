using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Data;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Services;
using SmartcatPlugin.Smartcat;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/basket")]
    public class BasketController : ApiController
    {
        private readonly IBasketService _basketService;
        private readonly Database _masterDb = Database.GetDatabase("master");
        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

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

        [Route("save-project-info")]
        [HttpPost]
        public IHttpActionResult SaveProjectInfo([FromBody] SaveProjectInfoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Fill required fields");
            }
            var projectInfo = JsonConvert.SerializeObject(dto);
            CustomCacheManager.SetCache("projectInfo", projectInfo);

            return Ok();
        }

        [Route("get-saved-project-info")]
        [HttpGet]
        public IHttpActionResult GetSavedProjectInfo()
        {
            string cachedData = CustomCacheManager.GetCache("projectInfo");
            if (string.IsNullOrEmpty(cachedData))
            {
                return Ok(new List<string>()); // todo: exception
            }

            var projectInfo = JsonConvert.DeserializeObject<SaveProjectInfoDto>(cachedData);

            return Ok(projectInfo);
        }

        [Route("get-translation-languages")]
        [HttpGet]
        public IHttpActionResult GetTranslationLanguages()
        {
            var result = new TranslationLanguagesDto
            {
                SourceLanguages = new List<LanguageDto>
                {
                    new LanguageDto
                    {
                        Name = "English", Code = "en"
                    },
                    new LanguageDto
                    {
                        Name = "Russian", Code = "ru"
                    }
                },
                TargetLanguages = new List<LanguageDto>
                {
                    new LanguageDto
                    {
                        Name = "English", Code = "en"
                    },
                    new LanguageDto
                    {
                        Name = "Russian", Code = "ru"
                    },
                    new LanguageDto
                    {
                        Name = "Spanish", Code = "es"
                    },
                    new LanguageDto
                    {
                        Name = "French", Code = "fr"
                    },
                    new LanguageDto
                    {
                        Name = "German", Code = "de"
                    },
                }
            };

            return Ok(result);
        }

        [Route("save-project")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateSmartcatProject([FromBody] ProjectDto dto)
        {
            //var basketService = new BasketService();
            var user = Sitecore.Context.User.Name;              //todo: cached data by user name
            string cachedData = CustomCacheManager.GetCache("selectedItems");
            var selectedItemIds = JsonConvert.DeserializeObject<List<string>>(cachedData);
            var items = _basketService.GetItemsByIds(_masterDb, selectedItemIds, dto.SourceLanguage);

            var client = new SmartcatApiClient();
            var response = await client.CreateProject(dto);

            if (!response.IsSuccess)
            {
                return BadRequest("Project creating failed");
            }

            var documentDtos = new List<DocumentDto>();

            foreach (var item in items)
            {
                var itemContent = item.GetItemContent(_masterDb, new [] { dto.TargetLanguage });

                var documentDto = new DocumentDto
                {
                    WorkSpaceId = dto.WorkspaceId,
                    ProjectId = response.Data.ProjectId,
                    Title = item.Name,
                    Content = itemContent.Values.First()
                };
                documentDtos.Add(documentDto);
            }

            var results = await client.CreateDocuments(documentDtos);

            if (results.Any(r => !r.IsSuccess))
            {
                var failedDocumentCount = results.Count(r => !r.IsSuccess);
                return BadRequest($"{failedDocumentCount}th document was failed");
            }

            return Ok();
        }
    }
}