using log4net;
using SmartcatPlugin.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using static Sitecore.ContentSearch.Linq.Extensions.ReflectionExtensions;

namespace SmartcatPlugin.Models.ApiResponse
{
    public class ApiResponseBase
    {
        private static readonly ILog Log = LogManager.GetLogger(LogNames.SmartcatApi);
        public bool IsSuccess { get; set; }
        public string StackTrace { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public static ApiResponse<T> Success = new ApiResponse<T> { IsSuccess = true };

        public static ApiResponse<object> Error(HttpStatusCode errorCode, string errorMessage)
        {
            string stackTrace = Environment.StackTrace;

            Log.Error($"{errorMessage} Called from: {stackTrace}");

            return new ApiResponse<object>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                StatusCode = errorCode,
                StackTrace = stackTrace,
                Data = null
            };
        }
    }
}