using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using SmartcatPlugin.Interfaces;

namespace SmartcatPlugin
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ISmartcatLoggingService _logger;

        public CustomExceptionFilter(ISmartcatLoggingService logger)
        {
            _logger = logger;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _logger.LogError("An unexpected error occurred", context.Exception);

            var response = new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred. Please try again later.",
                Error = context.Exception.Message,
                context.Exception.StackTrace
            };

            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, response);
        }
    }
}