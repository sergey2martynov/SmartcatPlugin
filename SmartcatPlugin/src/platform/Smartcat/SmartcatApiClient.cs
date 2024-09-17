using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using System.Text;
using SmartcatPlugin.Services;
using Sitecore.Data;
using System.Security.Policy;
using System.Threading;
using System.Net;

namespace SmartcatPlugin.Smartcat
{
    public class SmartcatApiClient
    {
        private static readonly HttpClient _httpClient;

        static SmartcatApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://ihub.smartcat.com");
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

        public async Task<ApiResponse<ProjectIdDto>> CreateProject(ProjectDto dto)
        {
            var authService = new AuthService();
            var apiKey = authService.GetApiKey(Database.GetDatabase("master"));
            dto.WorkspaceId = apiKey.WorkspaceId;
            dto.IntegrationType = "sitecore-app";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    EncodeClientIdSecretToBase64(apiKey.WorkspaceId, apiKey.ApiKey));
            var response = await _httpClient.PostAsync("/api/v1/projects", CreateJsonContent(dto));
            var result = await HandleResponse<ProjectIdDto>(response);
            return result;
        }

        public async Task<List<ApiResponse<DocumentIdDto>>> CreateDocuments(List<DocumentDto> documentDto)
        {
            var result = new List<ApiResponse<DocumentIdDto>>();
            var semaphore = new SemaphoreSlim(10);

            var tasks = documentDto.Select(async dto =>
            {
                await semaphore.WaitAsync();

                try
                {
                    return await SendWithRetry(dto);
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

        private async Task<ApiResponse<DocumentIdDto>> SendWithRetry(DocumentDto dto)
        {
            int maxRetries = 3;
            int delay = 1000;

            for (int i = 0; i < maxRetries; i++)
            {
                var response = await _httpClient.PostAsync("/api/v1/documents", CreateJsonContent(dto));

                if (response.IsSuccessStatusCode)
                {
                    return await HandleResponse<DocumentIdDto>(response);
                }

                if ((int)response.StatusCode == 429)
                {
                    await Task.Delay(delay);
                    delay *= 2;
                }
                else
                {

                    return await HandleResponse<DocumentIdDto>(response);
                }
            }

            return new ApiResponse<DocumentIdDto>
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

        private static HttpClient CreateClient()
        {
            var authService = new AuthService();
            var apiKey = authService.GetApiKey(Database.GetDatabase("master"));
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                "Basic " + EncodeClientIdSecretToBase64(apiKey.WorkspaceId, apiKey.ApiKey));
            return httpClient;
        }

        private static string EncodeClientIdSecretToBase64(string workspace, string apiKey)
        {
            var encoding = Encoding.UTF8;
            var toEncode = workspace + ":" + apiKey;
            return Convert.ToBase64String(encoding.GetBytes(toEncode));
        }
    }
}