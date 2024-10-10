using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.SmartcatApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartcatPlugin.Interfaces
{
    public interface ISmartcatApiClient
    {
        Task<ApiResponse<ProjectListDto>> GetProjects(GetProjectListRequest request);
        Task<ApiResponse<object>> ValidateApiKeyAsync(ApiKeyDto dto);
        Task<ApiResponse<ProjectIdDto>> CreateProject(CreateProjectDto dto);
        Task<ApiResponse<GetDocumentsByProjectIdResponse>> GetDocumentsByProjectId(
            GetDocumentsByProjectIdRequest request);

        Task<List<ApiResponse<TResponse>>> SendRequests<TRequest, TResponse>(List<TRequest> requestDtos,
            string endpoint, HttpMethod method);
    }
}
