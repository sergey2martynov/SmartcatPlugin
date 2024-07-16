using System;
using log4net;
using SmartcatPlugin.Constants;

namespace SmartcatPlugin.Models.ApiResponse
{
    public class ApiResponse
    {
        private static readonly ILog Log = LogManager.GetLogger(LogNames.SmartcatApi);
        public bool IsSuccess { get; set; }
        public string StackTrace { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }

        public static ApiResponse Success = new ApiResponse { IsSuccess = true };

        public static ApiResponse Error(int errorCode, string errorMessage)
        {
            string stackTrace = Environment.StackTrace;

            Log.Error($"{errorMessage} Called from: {stackTrace}");

            return new ApiResponse
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                StackTrace = stackTrace
            };
        }

    }
}