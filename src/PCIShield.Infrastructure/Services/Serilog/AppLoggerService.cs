using System;

using ILogger = Serilog.ILogger;
namespace PCIShield.Infrastructure.Services
{
    public class AppLoggerService<T> : IAppLoggerService<T>
    {
        private readonly ILogger _logger;
        public AppLoggerService(ILogger logger)
        {
            _logger = logger.ForContext<T>();
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

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }
        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }
        public void LogError(string message, string error, params object[] args)
        {
            _logger.Error(message, args);
        }
        public void LogError(Exception ex, string v)
        {
            _logger.Error(ex, v);
        }
    }
    public static class LoggerExtensions
    {
        public static IAppLoggerService<TTarget> ForType<TTarget>(this IAppLoggerService anyLogger)
        {
            var serilogLogger = Serilog.Log.Logger;
            return new AppLoggerService<TTarget>(serilogLogger);
        }
        public static IAppLoggerService<TTarget> ForType<TTarget>(this IAppLoggerService anyLogger, ILogger serilogLogger)
        {
            return new AppLoggerService<TTarget>(serilogLogger);
        }
    }
}