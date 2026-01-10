namespace PCIShield.Client.Services.Common
{
    public interface IAppLoggerService<T>
    {
        void LogError(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);

        void LogError(Exception ex, string v);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
    }
}