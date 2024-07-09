namespace SmartcatPlugin.Models.ApiResponse
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }

        public static ApiResponse Success = new ApiResponse { IsSuccess = true };
        public static ApiResponse Error = new ApiResponse { IsSuccess = false };

        public static ApiResponse Return(int errorCode, string errorMessage)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }

    }
}