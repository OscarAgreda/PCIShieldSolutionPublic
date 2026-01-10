using ILogger = Serilog.ILogger;
namespace PCIShield.Client.Services.Common
{
    public class AppLoggerService<T> : IAppLoggerService<T>
    {
        private readonly ILogger _logger;
        public AppLoggerService(ILogger logger)
        {
            _logger = logger.ForContext<T>();
        }
        public void LogError(string message, params object[] args)
        {
            try
            {
                _logger.Error(message, args);
            }
            catch (Exception ex)
            {
            }
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            try
            {
                _logger.Error(message, args);
            }
            catch (Exception ex1)
            {
            }
        }
        public void LogError(Exception ex, string v)
        {
            _logger.Error(ex, v);
        }
        public void LogInformation(string message, params object[] args)
        {
            try
            {
                _logger.Information(message, args);
            }
            catch (Exception ex)
            {
            }
        }
        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }
    }
}