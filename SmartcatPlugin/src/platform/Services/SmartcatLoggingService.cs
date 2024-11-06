using log4net;
using System;
using System.Threading.Tasks;
using Sitecore;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Interfaces;
using Sitecore.Data;

namespace SmartcatPlugin.Services
{
    public class SmartcatLoggingService : ISmartcatLoggingService
    { 
        private static readonly ILog Log = LogManager.GetLogger(StringConstants.SmartcatLogger);
        private readonly Database _masterDb = Database.GetDatabase("master");

        public void LogInfo(string message)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(message);
            }
        }

        public void LogError(string message, Exception ex = null)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(message, ex);
            }
        }

        public async Task UpdateContextItem()
        {
            await Task.Delay(4000);
            Sitecore.Data.Items.Item myItem = _masterDb.GetItem("/sitecore/content"); ; //TODO: set to the appropriate item
            string load = String.Concat(new object[] { "item:load(id=", myItem.ID, ",language=", myItem.Language, ",version=", myItem.Version, ")" });
            Sitecore.Context.ClientPage.SendMessage(this, load);
            String refresh = $"item:refreshchildren(id={myItem.Parent.ID})";
            Sitecore.Context.ClientPage.ClientResponse.Timer(refresh, 2);
        }
    }
}