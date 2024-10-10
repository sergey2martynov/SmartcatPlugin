using log4net;
using System;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Interfaces;

namespace SmartcatPlugin.Services
{
    public class SmartcatLoggingService : ISmartcatLoggingService
    { 
        private static readonly ILog Log = LogManager.GetLogger(StringConstants.SmartcatLogger);

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
    }
}