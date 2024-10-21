using SmartcatPlugin.Models.SmartcatApi.Base;

namespace SmartcatPlugin.Models.ApiResponse
{
    public class ApiResponse<T> : ApiResponseBase
        where T : ResponseData
    {
        public T Data { get; set; }
    }
}