using System;

namespace SmartcatPlugin.Interfaces
{
    public interface ISmartcatLoggingService
    {
        void LogInfo(string message);
        void LogError(string message, Exception ex = null);
    }
}
