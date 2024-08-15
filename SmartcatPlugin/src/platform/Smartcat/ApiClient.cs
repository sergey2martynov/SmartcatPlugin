using System.Net.Http;
using System;
using System.Configuration;

namespace SmartcatPlugin.Smartcat
{
    public class ApiClient
    {
        private static readonly HttpClient _httpClient;

        static ApiClient()
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }


    }
}