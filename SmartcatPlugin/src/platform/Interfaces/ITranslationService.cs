using SmartcatPlugin.Models;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.SmartcatApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartcatPlugin.Interfaces
{
    public interface ITranslationService
    {
        Task<List<ApiResponse<GetExportIdResponse>>> GetExportIds(string projectId, List<string> documentIds, string workSpaceId);
        Task<List<ApiResponse<GetItemTranslationResponse>>> GetTranslatedContent(List<string> exportIds,
            string workspaceId);
        void AddNewItemLanguageVersions(LocJsonContent content);
    }
}
