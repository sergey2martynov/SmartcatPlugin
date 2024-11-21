using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using System.Text;
using System.Threading;
using SmartcatPlugin.Constants;
using System.Web;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models.SmartcatApi;
using SmartcatPlugin.Models.SmartcatApi.Base;
using Smartcat.IntegrationHub.ApiClient;
using Sitecore.Data;
using Smartcat.IntegrationHub.Contracts.Dto.General;
using Smartcat.IntegrationHub.Contracts.Dto.Projects;
using DeleteProjectRequest = SmartcatPlugin.Models.SmartcatApi.DeleteProjectRequest;
using Smartcat.IntegrationHub.Contracts.Dto.Connections;
using Smartcat.IntegrationHub.Contracts.Dto.Shared;
using System.Dynamic;
using Smartcat.IntegrationHub.Contracts.Dto.DataItems;
using Smartcat.IntegrationHub.Contracts.Dto.DataDirectories;
using static System.Net.WebRequestMethods;

namespace SmartcatPlugin.Smartcat
{
    public class SmartcatApiClient : ISmartcatApiClient
    {
        private readonly ISmartcatLoggingService _logger;
        private const string Host = "https://ihub-us.smartcat.com/";

        private static WorkspacesApiClient _workspacesApiClient = new(HttpClient);
        private static DataItemApiClient _dataItemApiClient = new(HttpClient);
        private static DataDirectoryApiClient _dataDirectoryApiClient = new(HttpClient);
        private static ConnectionApiClient _connectionApiClient = new(HttpClient);
        private static ProjectsApiClient _projectsApiClient = new(HttpClient);

        private const string ConnectionName = "test connection sitecore";
        private Guid _connectionId;
        private static HttpClient HttpClient =>
            _httpClient ??= GetHttpClient();

        private static HttpClient _httpClient;
        private readonly IAuthService _authService;
        public SmartcatApiClient(ISmartcatLoggingService logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        private static HttpClient GetHttpClient()
        {
            var apiKey = GetApiKey();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " 
                                                                  + EncodeClientIdSecretToBase64(apiKey.WorkspaceId, apiKey.ApiKey));
            httpClient.BaseAddress = new Uri(Host);
            return httpClient;
        }

        public static ApiKeyDto GetApiKey()
        {
            var apiKeyItem = Database.GetDatabase("master").GetItem(ConstantIds.ApiKeyItem);

            var apiKey = new ApiKeyDto
            {
                WorkspaceId = apiKeyItem.Fields[StringConstants.WorkSpaceId].Value,
                ApiKey = apiKeyItem.Fields[StringConstants.ApiKey].Value
            };
            return apiKey;
        }

        private async Task EnsureConnection()
        {
            if (_connectionId != Guid.Empty)
            {
                return;
            }

            var apiKey = GetApiKey();
            var connectionsClient = _connectionApiClient;

            try
            {
                var connections2 = await connectionsClient.GetConnectionsAsync(apiKey.WorkspaceId, false);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }

            var connections = await connectionsClient.GetConnectionsAsync(apiKey.WorkspaceId, false);

            var connectionId = connections.Connections.FirstOrDefault(x =>
                x.ConnectionTypeId == Hub20IntegrationTypes.LokaliseV2)?.Id;

            if (connectionId == null)
            {
                IDictionary<string, object> properties = new ExpandoObject();
                properties["ApiKey"] = "c34e98e24877961a27abed334e7d8c1301f385ed";
                properties["Url"] = "https://sc10sc.dev.local/";
                var createdConnection = await connectionsClient.CreateAsync(new CreateConnectionRequest
                {
                    ConnectionName = ConnectionName,
                    IsHidden = true,
                    ConnectionTypeId = Hub20IntegrationTypes.Sitecore,
                    ScWorkspaceId = apiKey.WorkspaceId,
                    Properties = (ExpandoObject)properties
                });

                connectionId = createdConnection.Id;
            }

            _connectionId = connectionId.Value;
        }

        public async Task<bool> ValidateApiKeyAsync(ApiKeyDto dto)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic "
                                                                  + EncodeClientIdSecretToBase64(dto.WorkspaceId, dto.ApiKey));
            httpClient.BaseAddress = new Uri(Host);
            _workspacesApiClient = new (httpClient);
            _dataItemApiClient = new (httpClient);
            _dataDirectoryApiClient = new (httpClient);
            _connectionApiClient = new (httpClient);
            _projectsApiClient = new (httpClient);

            var response = await _workspacesApiClient.ValidateApiKeyAsync(dto.WorkspaceId,
                new ValidateApiKeyRequest { ApiKey = dto.ApiKey });
            return response.IsValid;
        }

        public async Task<string> CreateProject(CreateProjectRequest request)
        {
            await EnsureConnection();
            var createdProject = await _projectsApiClient.CreateProjectAsync(_connectionId,
                new CreateProjectForConnectionRequest
                {
                    Name = request.Name,
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguages = request.TargetLanguages.ToArray(),
                    DueDate = request.DueDate == DateTime.MinValue ? null : request.DueDate,
                    ProjectTemplateId = request.ProjectTemplateId,
                    Stages = (ProjectStages)request.Stage

                });
            return createdProject.Id;
        }

        public async Task<DataItemInfo[]> CreateDocuments(List<CreateDocumentRequest> requests, string sourceLanguage)
        {
            var semaphore = new SemaphoreSlim(10);

            var directoryId = new ExternalObjectId(Guid.NewGuid().ToString(), "lokalise-project");

            await _dataDirectoryApiClient.CreateDataDirectoryAsync(_connectionId,
                new CreateDataDirectoryRequest
                {
                    Title = "Test dir",
                    ExternalDirectoryId = directoryId,
                    ParentId = ExternalObjectId.Root,
                });

            var tasks = requests.Select(async request =>
            {
                await semaphore.WaitAsync();

                try
                {
                    return await _dataItemApiClient.ImportDataItemAsync(
                        _connectionId,
                        new ImportDataItemRequest
                        {
                            ProjectId = Guid.Parse(request.ProjectId),
                            ExternalItemId = request.ExternalObjectId,
                            ParentIds = Array.Empty<ExternalObjectId>(),
                            Title = request.Title,
                            Content = request.Content,
                            SourceLanguage = sourceLanguage,
                            TargetLanguage = request.TargetLanguage,
                        });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            try
            {
                var responses2 = await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var responses = await Task.WhenAll(tasks);
            return responses;
        }

        public async Task<ApiResponse<GetProjectListResponse>> GetProjects(GetProjectListRequest request)
        {
            FillHttpClientAuthHeaders();

            var builder = new UriBuilder(_httpClient.BaseAddress + "api/v1/projects");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["workspaceId"] = request.WorkspaceId;
            query["limit"] = NumberConstants.BatchSize.ToString();
            query["offset"] = request.Offset.ToString();
            builder.Query = query.ToString();

            var response = await _httpClient.GetAsync(builder.ToString());
            var result = await HandleResponse<GetProjectListResponse>(response);
            return result;
        }

        public async Task<ApiResponse<GetDocumentsByProjectIdResponse>> GetDocumentsByProjectId(GetDocumentsByProjectIdRequest request)
        {
            var response = await _httpClient.GetAsync($"/api/v1/documents?workspaceId={request.WorkspaceId}" +
                                                      $"&projectId={request.ProjectId}");
            
            var result = await HandleResponse<GetDocumentsByProjectIdResponse>(response);
            return result;
        }

        

        /*public async Task<ApiResponse<CreateProjectResponse>> CreateProject(CreateProjectRequest request)
        {
            var apiKey = _authService.GetApiKey();
            request.WorkspaceId = apiKey.WorkspaceId;
            request.IntegrationType = "sitecore-app";
            FillHttpClientAuthHeaders();

            //
            var tempRequest = new CreateProjectRequestRequest
            {
                IntegrationType = request.IntegrationType,
                WorkspaceId = apiKey.WorkspaceId,
                Name = request.Name,
                Description = "desc",
                SourceLanguage = request.SourceLanguage,
                TargetLanguage = request.TargetLanguages[0],
                DueDate = null,
                ProjectTemplateId = request.ProjectTemplateId,
                SelectedItemIds = request.SelectedItemIds
            };
            //
            var response = await _httpClient.PostAsync("/api/v1/projects", CreateJsonContent(tempRequest));
            var result = await HandleResponse<CreateProjectResponse>(response);
            return result;
        }*/

        public async Task<ApiResponse<GetItemTranslationResponse>> GetItemTranslation(GetItemTranslationRequest request)
        {
            var response = await _httpClient.PostAsync("/api/v1/documents/export-status", CreateJsonContent(request));
            var result = await HandleResponse<GetItemTranslationResponse>(response);
            return result;
        }

        public async Task<ApiResponse<GetTemplateResponse>> GetTemplates(GetTemplatesRequest request)
        {
            FillHttpClientAuthHeaders();
            var response = await _httpClient.PostAsync("/api/v1/workspaces/template-list", CreateJsonContent(request));
            var result = await HandleResponse<GetTemplateResponse>(response);
            return result;
        }

        public async Task<ApiResponse<ResponseData>> DeleteProject(DeleteProjectRequest request)
        {
            FillHttpClientAuthHeaders();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/projects");
            var content = JsonConvert.SerializeObject(request);
            httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);
            var result = await HandleResponse<ResponseData>(httpResponseMessage);
            return result;
        }

        public async Task<ApiResponse<ResponseData>> DeleteDocument(DeleteDocumentRequest request)
        {
            FillHttpClientAuthHeaders();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/documents");
            var content = JsonConvert.SerializeObject(request);
            httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);
            var result = await HandleResponse<ResponseData>(httpResponseMessage);
            return result;
        }

        public async Task<List<ApiResponse<TResponse>>> SendRequests<TRequest, TResponse>(List<TRequest> requestDtos,
            string endpoint, HttpMethod method) where TResponse : ResponseData
        {
            if (requestDtos == null || !requestDtos.Any())
            {
                throw new ArgumentException("RequestDtos parameter cant be null or empty");
            }

            FillHttpClientAuthHeaders();
            var result = new List<ApiResponse<TResponse>>();
            var semaphore = new SemaphoreSlim(10);

            var tasks = requestDtos.Select(async dto =>
            {
                await semaphore.WaitAsync();

                try
                {
                    
                    return await SendRequest<TRequest, TResponse>(dto, endpoint, method);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var responses = await Task.WhenAll(tasks);
            result.AddRange(responses);

            return result;
        }

        private async Task<ApiResponse<TResponse>> SendRequest<TRequest, TResponse>(TRequest request, string endpoint,
            HttpMethod method, int maxRetries = 3, int delay = 1000)
            where TResponse : ResponseData
        {
            for (int i = 0; i < maxRetries; i++)
            {
                var httpRequestMessage = new HttpRequestMessage(method, endpoint);
                var json = JsonConvert.SerializeObject(request);
                httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInfo($"Calling smartcat API, method: {httpRequestMessage.Method} endpoint: {httpRequestMessage.RequestUri}," +
                                $" requestData: {httpRequestMessage.Content}.");

                var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);
                var response = await HandleResponse<TResponse>(httpResponseMessage);

                if (response.IsSuccess && !response.Data.IsValid())
                {
                    _logger.LogInfo($"Retrying API call. Attempt {i}.");
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                if ((int)response.StatusCode == NumberConstants.ToManyRequests)
                {
                    _logger.LogInfo($"Retrying API call. Attempt {i}.");
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                if (!response.IsSuccess)
                {
                    _logger.LogInfo($"Retrying API call. Attempt {i}.");
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }
                
                return response;
            }

            return new ApiResponse<TResponse>
            {
                IsSuccess = false,
                ErrorMessage = "Failed after retrying due to TooManyRequests"
            };
        }

        private StringContent CreateJsonContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
            where T : ResponseData
        {
            _logger.LogInfo($"Smartcat API response: StatusCode = {response.StatusCode}, Content = {response.Content}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(content);
                return new ApiResponse<T>
                {
                    Data = data,
                    IsSuccess = true,
                    StatusCode = response.StatusCode
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    StatusCode = response.StatusCode,
                    ErrorMessage = errorContent
                };
            }
        }

        private static string EncodeClientIdSecretToBase64(string workspace, string apiKey)
        {
            var encoding = Encoding.UTF8;
            var toEncode = workspace + ":" + apiKey;
            return Convert.ToBase64String(encoding.GetBytes(toEncode));
        }

        private void FillHttpClientAuthHeaders()
        {
            var apiKey = _authService.GetApiKey();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    EncodeClientIdSecretToBase64(apiKey.WorkspaceId, apiKey.ApiKey));
        }
    }
}