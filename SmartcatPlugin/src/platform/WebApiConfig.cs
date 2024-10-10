using Microsoft.Extensions.DependencyInjection;
using SmartcatPlugin.Interfaces;
using System.Web.Http;

namespace SmartcatPlugin
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var logger = Sitecore.DependencyInjection.ServiceLocator.ServiceProvider.GetService<ISmartcatLoggingService>();
            config.Filters.Add(new CustomExceptionFilter(logger));
        }
    }
}