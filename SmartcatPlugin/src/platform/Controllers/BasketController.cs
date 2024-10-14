using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Sitecore.Data;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.Dtos;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/basket")]
    public class BasketController : ApiController
    {
        private readonly IBasketService _basketService;
        private readonly ISmartcatApiClient _apiClient;
        private readonly ICacheService _cacheService;
        private readonly IItemService _itemService;
        private readonly Database _masterDb = Database.GetDatabase("master");

        public BasketController(IBasketService basketService,
            ISmartcatApiClient apiClient,
            ICacheService cacheService,
            IItemService itemService)
        {
            _basketService = basketService;
            _apiClient = apiClient;
            _cacheService = cacheService;
            _itemService = itemService;
        }

        [Route("get-selected-items")]
        [HttpGet]
        public IHttpActionResult GetSelectedItems()
        {
            var result = _basketService.BuildSelectedItemTree();

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
            var userName = Sitecore.Context.User.Name;
            string cachedData = _cacheService.GetValue($"{userName}:{StringConstants.SelectedItems}");
            if (string.IsNullOrEmpty(cachedData))
            {
                return Ok(new List<string>()); // todo: exception
            }

            var itemIds = JsonConvert.DeserializeObject<List<string>>(cachedData);
            var invalidItemNames = _itemService.GetInvalidItemsNames(itemIds);

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

            var userName = Sitecore.Context.User.Name;
            var projectInfo = JsonConvert.SerializeObject(dto);
            _cacheService.SetValue($"{userName}:{StringConstants.ProjectInfo}", projectInfo);

            return Ok();
        }

        [Route("get-saved-project-info")]
        [HttpGet]
        public IHttpActionResult GetSavedProjectInfo()
        {
            var userName = Sitecore.Context.User.Name;
            string cachedData = _cacheService.GetValue($"{userName}:{StringConstants.ProjectInfo}");
            if (string.IsNullOrEmpty(cachedData))
            {
                return BadRequest("Cash was now found");
            }

            var projectInfo = JsonConvert.DeserializeObject<SaveProjectInfoDto>(cachedData);

            return Ok(projectInfo);
        }

        [Route("get-translation-languages")]
        [HttpGet]
        public IHttpActionResult GetTranslationLanguages()
        {
            var defaultLanguage = _basketService.GetDefaultLanguage();
            var targetLanguages = _basketService.GetAvailableLanguages();

            var result = new TranslationLanguagesDto
            {
                SourceLanguages = new List<LanguageDto>
                {
                    defaultLanguage
                },
                TargetLanguages = targetLanguages
            };

            return Ok(result);
        }

        [Route("save-project")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateSmartcatProject([FromBody] CreateProjectDto dto)
        {
            var userName = Sitecore.Context.User.Name;
            string cachedData = _cacheService.GetValue($"{userName}:{StringConstants.SelectedItems}");
            var selectedItemIds = JsonConvert.DeserializeObject<List<string>>(cachedData);
            var response = await _apiClient.CreateProject(dto);

            if (!response.IsSuccess)
            {
                return BadRequest("Project creating failed");
            }

            var documentDtos = new List<DocumentDto>();
            var items = _basketService.GetItemsByIds(_masterDb, selectedItemIds, dto.SourceLanguage);

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

            var results = await _apiClient
                .SendRequests<DocumentDto, DocumentIdDto>(documentDtos, "/api/v1/documents" ,
                    HttpMethod.Post).ConfigureAwait(false);

            if (results.Exists(r => !r.IsSuccess))
            {
                var failedDocumentCount = results.Count(r => !r.IsSuccess);
                return BadRequest($"{failedDocumentCount}th document was failed");
            }

            return Ok();
        }
    }
}