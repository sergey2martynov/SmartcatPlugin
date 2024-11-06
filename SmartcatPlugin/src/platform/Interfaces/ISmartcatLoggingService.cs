using System;
using System.Threading.Tasks;

namespace SmartcatPlugin.Interfaces
{
    public interface ISmartcatLoggingService
    {
        void LogInfo(string message);
        void LogError(string message, Exception ex = null);
        Task UpdateContextItem();
    }
}
