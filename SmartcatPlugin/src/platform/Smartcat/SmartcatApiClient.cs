using System.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace SmartcatPlugin.Smartcat
{
    public class SmartcatApiClient : ISmartcatApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ISmartcatLoggingService _logger;
        private readonly IAuthService _authService;

        public SmartcatApiClient(ISmartcatLoggingService logger, IAuthService authService)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://ihub.smartcat.com");
            _logger = logger;
            _authService = authService;
        }

        public async Task<ApiResponse<ProjectListDto>> GetProjects(GetProjectListRequest request)
        {
            FillHttpClientAuthHeaders();

            var builder = new UriBuilder(_httpClient.BaseAddress + "api/v1/projects");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["workspaceId"] = request.WorkspaceId;
            query["limit"] = NumberConstants.BatchSize.ToString();
            query["offset"] = request.Offset.ToString();
            builder.Query = query.ToString();

            var response = await _httpClient.GetAsync(builder.ToString());
            var result = await HandleResponse<ProjectListDto>(response);
            return result;
        }

        public async Task<ApiResponse<GetDocumentsByProjectIdResponse>> GetDocumentsByProjectId(GetDocumentsByProjectIdRequest request)
        {
            var response = await _httpClient.GetAsync($"/api/v1/documents?workspaceId={request.WorkspaceId}" +
                                                      $"&projectId={request.ProjectId}");
            
            var result = await HandleResponse<GetDocumentsByProjectIdResponse>(response);
            return result;
        }

        public async Task<ApiResponse<object>> ValidateApiKeyAsync(ApiKeyDto dto)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    EncodeClientIdSecretToBase64(dto.WorkspaceId, dto.ApiKey));
            var response = await _httpClient.PostAsync("/api/v1/workspaces/validate-api-key", CreateJsonContent(dto));
            var result = await HandleResponse<object>(response);
            return result;
        }

        public async Task<ApiResponse<ProjectIdDto>> CreateProject(CreateProjectDto dto)
        {
            var apiKey = _authService.GetApiKey();
            dto.WorkspaceId = apiKey.WorkspaceId;
            dto.IntegrationType = "sitecore-app";
            FillHttpClientAuthHeaders();
            var response = await _httpClient.PostAsync("/api/v1/projects", CreateJsonContent(dto));
            var result = await HandleResponse<ProjectIdDto>(response);
            return result;
        }

        public async Task<ApiResponse<GetItemTranslationResponse>> GetItemTranslation(GetItemTranslationRequest request)
        {
            var response = await _httpClient.PostAsync("/api/v1/documents/export-status", CreateJsonContent(request));
            var result = await HandleResponse<GetItemTranslationResponse>(response);
            return result;
        }

        public async Task<List<ApiResponse<TResponse>>> SendRequests<TRequest, TResponse>(List<TRequest> requestDtos,
            string endpoint, HttpMethod method)
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
                    var request = new HttpRequestMessage(method, endpoint);
                    var json = JsonConvert.SerializeObject(dto);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    return await SendRequest<TRequest, TResponse>(request);
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

        private async Task<ApiResponse<TResponse>> SendRequest<TRequest, TResponse>(HttpRequestMessage request)
        {
            int maxRetries = 3;
            int delay = 1000;

            for (int i = 0; i < maxRetries; i++)
            {
                _logger.LogInfo($"Calling smartcat API, method: {request.Method} endpoint: {request.RequestUri}," +
                                $" requestData: {request.Content}.");
                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var successResult = await HandleResponse<TResponse>(response);
                        return successResult;
                }

                if ((int)response.StatusCode == NumberConstants.ToManyRequests)
                {
                    _logger.LogInfo($"Retrying API call. Attempt {i}.");
                    await Task.Delay(delay);
                    delay *= 2;
                }
                else
                {
                    return await HandleResponse<TResponse>(response);
                }
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