﻿using Sitecore.Pipelines;
using System.Web.Http;
using SmartcatPlugin.API;

namespace SmartcatPlugin.Controllers
{
    public class RegisterPageRoutes : RegisterRoutesBase
    {
        public void Process(PipelineArgs args)
        {
            GlobalConfiguration.Configure(this.Configure);
        }

        protected void Configure(HttpConfiguration configuration)
        {
            MapRouteWithSession(configuration, "smartcat", "api/{action}", "Page", "index");
        }
    }
}