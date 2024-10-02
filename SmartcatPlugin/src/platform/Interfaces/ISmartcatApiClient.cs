using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Models.SmartcatApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartcatPlugin.Interfaces
{
    public interface ISmartcatApiClient
    {
        Task<ApiResponse<ProjectListDto>> GetProjects(GetProjectListRequest request);
        Task<ApiResponse<object>> ValidateApiKeyAsync(ApiKeyDto dto);
        Task<ApiResponse<ProjectIdDto>> CreateProject(CreateProjectDto dto);
        Task<List<ApiResponse<DocumentIdDto>>> CreateDocuments(List<DocumentDto> documentDtos);
        Task<ApiResponse<GetDocumentsByProjectIdResponse>> GetDocumentsByProjectId(
            GetDocumentsByProjectIdRequest request);
        Task<ApiResponse<GetExportIdResponse>> GetExportId(GetExportIdRequest request);
        Task<ApiResponse<GetItemTranslationResponse>> GetItemTranslation(GetItemTranslationRequest request);
    }
}
