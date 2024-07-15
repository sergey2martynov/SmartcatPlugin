using log4net;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using SmartcatPlugin.Constants;

namespace SmartcatPlugin
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private static readonly ILog Log = LogManager.GetLogger(LogNames.SmartcatApi);
        public override void OnException(HttpActionExecutedContext context)
        {
            Log.Error($"{context.Exception.Message}, {context.Exception.StackTrace}");

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