using log4net;
using System.Diagnostics;
using System.Reflection;
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
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(3).GetMethod();
            string methodName = $"{methodBase.ReflectedType?.Name}.{methodBase.Name}";

            Log.Error($"{errorMessage} Called from: {methodName}");

            return new ApiResponse
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                StackTrace = methodName
            };
        }

    }
}