using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.SmartcatApi;
using ExternalObjectId = Smartcat.IntegrationHub.Contracts.Dto.Shared.ExternalObjectId;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/basket")]
    public class BasketController : ApiController
    {
        private readonly IBasketService _basketService;
        private readonly ISmartcatApiClient _apiClient;
        private readonly IItemService _itemService;
        private readonly IAuthService _authService;

        public BasketController(IBasketService basketService,
            ISmartcatApiClient apiClient,
            IItemService itemService,
            IAuthService authService)
        {
            _basketService = basketService;
            _apiClient = apiClient;
            _itemService = itemService;
            _authService = authService;
        }

        [Route("get-translation-languages")]
        [HttpGet]
        public IHttpActionResult GetTranslationLanguages()
        {
            var defaultLanguage = _basketService.GetDefaultLanguage();
            var targetLanguages = _basketService.GetAvailableLanguages();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "sitecore modules/Shell/Smartcat/SmartcatLocales.json");
            var smartcatLanguages = new List<string>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                var jsonData = reader.ReadToEnd();
                smartcatLanguages = JsonConvert.DeserializeObject<List<string>>(jsonData);
            }

            var result = new TranslationLanguagesDto
            {
                SourceLanguages = new List<LanguageDto>
                {
                    defaultLanguage
                },
                TargetLanguages = targetLanguages,
                SmartcatLanguageCodes = smartcatLanguages
            };

            return Ok(result);
        }

        [Route("get-templates")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTemplates()
        {
            var apiKey = _authService.GetApiKey();
            var request = new GetTemplatesRequest
            {
                WorkSpaceId = apiKey.WorkspaceId
            };

            var result = await _apiClient.GetTemplates(request);

            return Ok(result.Data);
        }

        [Route("save-project")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateSmartcatProject([FromBody] CreateProjectRequest request)
        {
            var userName = Sitecore.Context.User.Name;
            var projectId = await _apiClient.CreateProject(request).ConfigureAwait(false);

            /*if (!response.IsSuccess)
            {
                return BadRequest("Project creating failed");
            }*/

            var createDocumentRequests = new List<CreateDocumentRequest>();
            var items = _basketService.GetItemsByIds(request.SelectedItemIds, request.SourceLanguage);

            foreach (var item in items)
            {
                var itemContent = _itemService.GetItemContent(item,  request.TargetLanguages );

                foreach (var content in itemContent)
                {
                    var createDocumentRequest = new CreateDocumentRequest
                    {
                        WorkSpaceId = request.WorkspaceId,
                        ProjectId = projectId,
                        Title = item.Name,
                        Content = content.Value,
                        ExternalObjectId = new ExternalObjectId(item.ID.ToString().Replace("{", "").Replace("}", ""), "lokalise-default-branch"),
                        TargetLanguage = content.Key
                    };

                    createDocumentRequests.Add(createDocumentRequest);
                }
            }

            var documents = await _apiClient.CreateDocuments(createDocumentRequests, request.SourceLanguage)
                .ConfigureAwait(false);
            /*var results = await _apiClient
                .SendRequests<CreateDocumentRequest, CreateDocumentResponse>(createDocumentRequests, "/api/v1/documents" ,
                    HttpMethod.Post).ConfigureAwait(false);

            if (results.Exists(r => !r.IsSuccess))
            {
                var failedDocumentCount = results.Count(r => !r.IsSuccess);
                return BadRequest($"{failedDocumentCount}th document was failed");
            }*/

            return Ok();
        }
    }
}