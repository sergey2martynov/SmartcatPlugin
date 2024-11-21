using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.SmartcatApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SmartcatPlugin.Models.SmartcatApi.Base;
using System;
using Smartcat.IntegrationHub.Contracts.Dto.DataItems;

namespace SmartcatPlugin.Interfaces
{
    public interface ISmartcatApiClient
    {
        Task<ApiResponse<GetProjectListResponse>> GetProjects(GetProjectListRequest request);
        Task<bool> ValidateApiKeyAsync(ApiKeyDto dto);
        Task<string> CreateProject(CreateProjectRequest request);
        Task<DataItemInfo[]> CreateDocuments(List<CreateDocumentRequest> requests, string sourceLanguage);
        Task<ApiResponse<GetItemTranslationResponse>> GetItemTranslation(GetItemTranslationRequest request);
        Task<ApiResponse<GetTemplateResponse>> GetTemplates(GetTemplatesRequest request);
        Task<ApiResponse<GetDocumentsByProjectIdResponse>> GetDocumentsByProjectId(
            GetDocumentsByProjectIdRequest request);
        Task<ApiResponse<ResponseData>> DeleteProject(DeleteProjectRequest request);
        Task<ApiResponse<ResponseData>> DeleteDocument(DeleteDocumentRequest request);
        Task<List<ApiResponse<TResponse>>> SendRequests<TRequest, TResponse>(List<TRequest> requestDtos,
            string endpoint, HttpMethod method) where TResponse : ResponseData;
    }
}
