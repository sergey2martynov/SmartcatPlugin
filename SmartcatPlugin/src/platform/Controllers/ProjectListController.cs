using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.SmartcatApi;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/project")]
    public class ProjectListController : ApiController
    {
        private readonly ISmartcatApiClient _apiClient;
        private readonly IAuthService _authService;
        private readonly ITranslationService _translationService;
        private readonly ISmartcatLoggingService _logging;

        public ProjectListController(ISmartcatApiClient apiClient,
            IAuthService authService,
            ITranslationService translationService,
            ISmartcatLoggingService logging
            )
        {
            _apiClient = apiClient;
            _authService = authService;
            _translationService = translationService;
            _logging = logging;
        }

        [Route("get-projects")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSmartcatProjects([FromUri] int currentPageNumber)
        {
            var apiKey = _authService.GetApiKey();

            if (string.IsNullOrEmpty(apiKey.WorkspaceId) || string.IsNullOrEmpty(apiKey.ApiKey))
            {
                throw new UnauthorizedAccessException("Access denied: WorkspaceId or ApiKey is missing or null.");
            }

            var request = new GetProjectListRequest
            {
                Offset = currentPageNumber * 10,
                WorkspaceId = apiKey.WorkspaceId
            };

            var result = await _apiClient.GetProjects(request).ConfigureAwait(false);

            var projectDtos = result.Data.Projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Status = p.Status == "inProgress" ? "In progress" : "Completed",
                Languages = $"{p.SourceLanguage} -> {string.Join(", ", p.TargetLanguages)}",
            });

            return Ok(new {projects = projectDtos });
        }

        [Route("get-documents")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDocuments(string projectId)
        {
            var apiKey = _authService.GetApiKey();
            var request = new GetDocumentsByProjectIdRequest
            {
                WorkspaceId = apiKey.WorkspaceId,
                ProjectId = projectId
            };

            var result = await _apiClient.GetDocumentsByProjectId(request);

            result.Data.Documents.ForEach(d => d.Status = d.Status == "inProgress" ? "In progress" : "Completed");

            return Ok(result.Data);
        }

        [Route("get-item-translations")]
        [HttpGet]
        public async Task<IHttpActionResult> GetItemTranslations([FromUri] string projectId)
        {
            _logging.LogInfo($"Translation process started for project with id:{projectId}");
            var workspaceId = _authService.GetWorkspaceId();
            var response = await _apiClient.GetDocumentsByProjectId(new GetDocumentsByProjectIdRequest
            {
                WorkspaceId = workspaceId,
                ProjectId = projectId
            });

            if (!response.IsSuccess)
            {
                throw new HttpRequestException($"{response.ErrorMessage}");
            }

            var documentIds = response.Data.Documents.Select(d => d.Id).ToList();
            var getExportIdsResponses =
                await _translationService.GetExportIds(projectId, documentIds, workspaceId).ConfigureAwait(false);

            int failedDocuments = 0;
            if (!getExportIdsResponses.All(e => e.IsSuccess))
            {
                failedDocuments = getExportIdsResponses.Count(e => !e.IsSuccess);
                getExportIdsResponses = getExportIdsResponses.Where(e => e.IsSuccess).ToList();
            }

            var exportIds = getExportIdsResponses.Select(r => r.Data.ExportId).ToList();

            var getTranslatedContentResponses =
                await _translationService.GetTranslatedContent(exportIds, workspaceId).ConfigureAwait(false);

            if (!getTranslatedContentResponses.All(e => e.IsSuccess))
            {
                failedDocuments += getTranslatedContentResponses.Count(r => !r.IsSuccess);
                getTranslatedContentResponses =
                    getTranslatedContentResponses.Where(e => e.IsSuccess).ToList();
            }

            var contentList = getTranslatedContentResponses
                .Select(r => r.Data.Content).ToList();

            foreach (var locJson in contentList)
            {
                _translationService.AddNewItemLanguageVersions(locJson);
            }

            var notification = $"Translation process ended for project with\nid:{projectId},\n" +
                               $" documents success:{documentIds.Count - failedDocuments},\n" +
                               $" failed{failedDocuments}";
            _logging.LogInfo(notification);
            return Ok(notification);
        }

        [Route("delete-project")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProject(string id)
        {
            var apiKey = _authService.GetApiKey();
            var request = new DeleteProjectRequest
            {
                WorkspaceId = apiKey.WorkspaceId,
                ProjectId = id
            };

            var result = await _apiClient.DeleteProject(request);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Route("delete-document")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteDocument(string id, string projectId)
        {
            var apiKey = _authService.GetApiKey();
            var request = new DeleteDocumentRequest
            {
                WorkspaceId = apiKey.WorkspaceId,
                ProjectId = projectId,
                DocumentId = id
            };

            var result = await _apiClient.DeleteDocument(request);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}