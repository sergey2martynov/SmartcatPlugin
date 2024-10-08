using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.SmartcatApi;

namespace SmartcatPlugin.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");
        private readonly ISmartcatApiClient _apiClient;
        private readonly IAuthService _authService;
        public TranslationService(ISmartcatApiClient apiClient,
            IAuthService authService
            )
        {
            _apiClient = apiClient;
            _authService = authService;
        }

        public async Task<List<ApiResponse<GetExportIdResponse>>> GetExportIds(string projectId, List<string> documentIds, string workspaceId)
        {
            var result = new List<ApiResponse<GetExportIdResponse>>();

            foreach (var documentId in documentIds)
            {
                var request = new GetExportIdRequest
                {
                    DocumentId = documentId,
                    ProjectId = projectId,
                    WorkspaceId = workspaceId
                };

                var response = await _apiClient.GetExportId(request);
                result.Add(response);
            }

            return result;
        }

        public async Task<List<ApiResponse<GetItemTranslationResponse>>> GetTranslatedContent(List<string> exportIds, string workspaceId)
        {
            await Task.Delay(5000);
            var result = new List<ApiResponse<GetItemTranslationResponse>>();

            foreach (var exportId in exportIds)
            {
                var request = new GetItemTranslationRequest()
                {
                    ExportId = exportId,
                    WorkspaceId = workspaceId
                };

                var response = await _apiClient.GetItemTranslation(request);
                result.Add(response);
            }

            return result;
        }

        public void AddNewItemLanguageVersions(LocJsonContent content)      //todo: make retry logic
        {
            if (content.Properties == null || content.Properties.TargetLanguage == null ||
                content.Properties.ItemId == null)
            {
                throw new ArgumentNullException(nameof(content.Properties), "Properties field cannot be null");
            }

            Language newLanguage = Language.Parse(content.Properties.TargetLanguage);
            var newVersionItemIds = new List<ID>();  // What is This?
            foreach (var unit in content.Units)
            {
                var fieldName = unit.Key;
                var childrenPageId = new ID(content.Properties.ItemId);
                var allVersionsItem = _masterDb.GetItem(childrenPageId);
                bool languageVersionExists = allVersionsItem.Languages.Any(l => l == newLanguage);

                var newLanguageItem = _masterDb.GetItem(childrenPageId, newLanguage);

                using (new SecurityDisabler())
                {
                    if (languageVersionExists && !newVersionItemIds.Contains(newLanguageItem.ID))  // What is This?
                    {
                        newLanguageItem = newLanguageItem.Versions.AddVersion();
                    }

                    newVersionItemIds.Add(newLanguageItem.ID);
                    newLanguageItem.Editing.BeginEdit();
                    newLanguageItem.Fields[fieldName].Value = string.Join(string.Empty, unit.Target);
                    newLanguageItem.Editing.EndEdit();
                }
            }
        }

        private async Task<ApiResponse<GetExportIdResponse>> SendRequestWithRetryAsync(GetExportIdRequest request)
        {
            int maxRetries = 3;
            int delay = 1000;

            for (int i = 0; i < maxRetries; i++)
            {
                var response = await _apiClient.GetExportId(request);

                if (response.IsSuccess)
                {
                    return response;
                }

                if ((int)response.StatusCode == 429)
                {
                    await Task.Delay(delay);
                    delay *= 2;
                }
                else
                {

                    return response;
                }
            }

            return new ApiResponse<GetExportIdResponse>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessage = "Failed after retrying due to TooManyRequests"
            };
        }
    }
}