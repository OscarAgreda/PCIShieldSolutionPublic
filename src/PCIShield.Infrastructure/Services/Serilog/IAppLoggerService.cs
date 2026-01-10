using System;
namespace PCIShield.Infrastructure.Services
{
    public interface IAppLoggerService
    {
        void LogInformation(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, string error, params object[] args);
        void LogError(Exception ex, string v);
    }
    public interface IAppLoggerService<T> : IAppLoggerService
    {
        void LogError(string message);
    }
}