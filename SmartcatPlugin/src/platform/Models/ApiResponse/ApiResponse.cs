using System;
using log4net;
using SmartcatPlugin.Constants;

namespace SmartcatPlugin.Models.ApiResponse
{
    public class ApiResponse<T> : ApiResponseBase
    {
        public T Data { get; set; }
    }
}