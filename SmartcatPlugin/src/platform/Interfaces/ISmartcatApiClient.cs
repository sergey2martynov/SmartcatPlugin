using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.SmartcatApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SmartcatPlugin.Models.SmartcatApi.Base;

namespace SmartcatPlugin.Interfaces
{
    public interface ISmartcatApiClient
    {
        Task<ApiResponse<GetProjectListResponse>> GetProjects(GetProjectListRequest request);
        Task<ApiResponse<ErrorResponse>> ValidateApiKeyAsync(ApiKeyDto dto);
        Task<ApiResponse<CreateProjectResponse>> CreateProject(CreateProjectRequest request);
        Task<ApiResponse<GetItemTranslationResponse>> GetItemTranslation(GetItemTranslationRequest request);
        Task<ApiResponse<GetDocumentsByProjectIdResponse>> GetDocumentsByProjectId(
            GetDocumentsByProjectIdRequest request);

        Task<List<ApiResponse<TResponse>>> SendRequests<TRequest, TResponse>(List<TRequest> requestDtos,
            string endpoint, HttpMethod method) where TResponse : ResponseData;
    }
}
