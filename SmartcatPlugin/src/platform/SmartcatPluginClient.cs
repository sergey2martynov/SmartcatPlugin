using Newtonsoft.Json;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Smartcat.GetFolderList;
using SmartcatPlugin.Models.Smartcat.GetItemById;
using SmartcatPlugin.Models.Smartcat.GetItemContent;
using SmartcatPlugin.Models.Smartcat.GetItemList;
using SmartcatPlugin.Models.Smartcat.GetParentDirectories;
using SmartcatPlugin.Models.Smartcat.ImportTranslation;
using SmartcatPlugin.Models.Smartcat.Testing;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartcatPlugin
{
    public class SmartcatPluginClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public SmartcatPluginClient(string baseAddress)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public async Task<GetFolderListResponse> GetFolderListAsync(GetFolderListRequest request)
        {
            var response = await _httpClient.PostAsync("directory-list", CreateJsonContent(request));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetFolderListResponse>(content);
        }

        public async Task<GetItemListResponse> GetItemListAsync(GetItemsListRequest request)
        {
            var response = await _httpClient.PostAsync("file-list", CreateJsonContent(request));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetItemListResponse>(content);
        }

        public async Task<GetItemContentResponse> GetItemContentAsync(GetItemContentRequest request)
        {
            var response = await _httpClient.PostAsync("file-content", CreateJsonContent(request));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetItemContentResponse>(content);
        }

        public async Task<ApiResponse> ImportTranslationAsync(TranslationImportRequest request)
        {
            var response = await _httpClient.PostAsync("import-translation", CreateJsonContent(request));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse>(content);
        }

        public async Task<GetParentFoldersResponse> GetParentFoldersAsync(GetParentFoldersRequest request)
        {
            var response = await _httpClient.PostAsync("parent-directories-by-id", CreateJsonContent(request));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetParentFoldersResponse>(content);
        }

        public async Task<GetItemByIdResponse> GetItemsByIdAsync(GetItemByIdRequest request)
        {
            var response = await _httpClient.PostAsync("files-by-id", CreateJsonContent(request));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetItemByIdResponse>(content);
        }

        public async Task<ApiResponse> CreateTestData()
        {
            var response = await _httpClient.PostAsync("test-data", null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse>(content);
        }

        public async Task<ApiResponse> DeleteTestData()
        {
            var response = await _httpClient.DeleteAsync("test-data");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse>(content);
        }

        public async Task<GetLocalesResponse> GetLocales()
        {
            var response = await _httpClient.PostAsync("locale-list", null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetLocalesResponse>(content);
        }

        private StringContent CreateJsonContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}