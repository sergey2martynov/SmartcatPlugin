using System.Web.Http;

namespace SmartcatPlugin
{
    public class WebApiInitializer
    {
        public void Process(Sitecore.Pipelines.PipelineArgs args)
        {
            /*var logConfigFile = HostingEnvironment.MapPath("~/App_Config/Log4net.config");
            if (logConfigFile != null && File.Exists(logConfigFile))
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(logConfigFile));
            }*/

            log4net.Config.XmlConfigurator.Configure();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}