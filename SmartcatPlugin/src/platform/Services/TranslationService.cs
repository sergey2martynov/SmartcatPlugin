using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Models;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.SmartcatApi;
using static Sitecore.Shell.Applications.Globalization.ExportLanguage.ExportLanguageForm;

namespace SmartcatPlugin.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly Database _masterDb = Database.GetDatabase("master");
        private readonly ISmartcatApiClient _apiClient;

        public TranslationService(ISmartcatApiClient apiClient
            )
        {
            _apiClient = apiClient;
        }

        public async Task<List<ApiResponse<GetExportIdResponse>>> GetExportIds(string projectId, List<string> documentIds,
            string workspaceId)
        {
            var requests = documentIds.Select(documentId => 
                new GetExportIdRequest
                {
                    DocumentId = documentId, 
                    ProjectId = projectId, 
                    WorkspaceId = workspaceId
                }).ToList();

            var responses = await _apiClient.SendRequests<GetExportIdRequest, GetExportIdResponse>(requests,
                "/api/v1/documents/export", HttpMethod.Post);

            return responses;
        }

        public async Task<List<ApiResponse<GetItemTranslationResponse>>> GetTranslatedContent(List<string> exportIds, string workspaceId)
        {
            if (exportIds == null || !exportIds.Any())
            {
                throw new ArgumentException("ExportIds parameter cant be null or empty");
            }

            var requests = exportIds.Select(exportId =>
                new GetItemTranslationRequest
                {
                    WorkspaceId = workspaceId,
                    ExportId = exportId
                }).ToList();

            var results = await _apiClient
                .SendRequests<GetItemTranslationRequest, GetItemTranslationResponse>(requests, "/api/v1/documents/export-status",
                    HttpMethod.Post).ConfigureAwait(false);

            return results;
        }

        public async Task<ApiResponse<GetItemTranslationResponse>> GetItemTranslation(GetItemTranslationRequest request)
        {
            int maxRetries = 3;
            int delay = 2000;

            for (int i = 0; i < maxRetries; i++)
            {
                var response = await _apiClient.GetItemTranslation(request);

                if (response.IsSuccess && response.Data.Content == null)
                {
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                if ((int)response.StatusCode == NumberConstants.ToManyRequests)
                {
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                if (!response.IsSuccess)
                {
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                return response;
            }

            return new ApiResponse<GetItemTranslationResponse> { IsSuccess = false, ErrorMessage = "Could not get response" };
        }

        public void AddNewItemLanguageVersions(LocJsonContent content)      //todo: make retry logic
        {
            if (content.Properties == null || content.Properties.TargetLanguage == null ||
                content.Properties.ItemId == null)
            {
                throw new ArgumentNullException(nameof(content.Properties), "Properties field cannot be null");
            }

            Language newLanguage = Language.Parse(content.Properties.TargetLanguage);
            var newVersionItemIds = new List<ID>();
            foreach (var unit in content.Units)
            {
                var fieldName = unit.Key;
                var childrenPageId = new ID(content.Properties.ItemId);
                var allVersionsItem = _masterDb.GetItem(childrenPageId);
                bool languageVersionExists = allVersionsItem.Languages.Any(l => l == newLanguage);

                var newLanguageItem = _masterDb.GetItem(childrenPageId, newLanguage);

                using (new SecurityDisabler())
                {
                    if (languageVersionExists && !newVersionItemIds.Contains(newLanguageItem.ID))
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
    }
}