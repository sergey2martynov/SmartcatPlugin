﻿using System.Web.Http;

namespace SmartcatPlugin
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new CustomExceptionFilter());
        }
    }
}