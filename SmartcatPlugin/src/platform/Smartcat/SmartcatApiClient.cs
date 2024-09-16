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
            using (HttpClient client = new HttpClient())
            {
                var tasks = documentDto.Select(dto => 
                    _httpClient.PostAsync("/api/v1/documents", CreateJsonContent(dto)));
                var httpResponseMessages = await Task.WhenAll(tasks);

                foreach (var httpResponseMessage in httpResponseMessages)
                {
                    var response = await HandleResponse<DocumentIdDto>(httpResponseMessage);
                    result.Add(response);
                }
            }

            return result;
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