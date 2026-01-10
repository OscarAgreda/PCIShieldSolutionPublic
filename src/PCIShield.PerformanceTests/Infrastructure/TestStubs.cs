using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.InvoiceSession;

namespace PCIShield.PerformanceTests.Infrastructure;

public class StubLogger<T> : IAppLoggerService<T>
{
    public void LogInformation(string message, params object[] args) 
    {
        if (Environment.GetEnvironmentVariable("NBOMBER_DEBUG") == "true")
            Console.WriteLine($"[INFO] {string.Format(message, args)}");
    }

    public void LogWarning(string message, params object[] args)
    {
        if (Environment.GetEnvironmentVariable("NBOMBER_DEBUG") == "true")
            Console.WriteLine($"[WARN] {string.Format(message, args)}");
    }

    public void LogError(string message, params object[] args)
        => Console.WriteLine($"[ERROR] {string.Format(message, args)}");

    public void LogError(Exception ex, string message, params object[] args)
        => Console.WriteLine($"[ERROR] {string.Format(message, args)} - Exception: {ex.Message}");

    public void LogError(Exception ex, string v)
        => Console.WriteLine($"[ERROR] {v} - Exception: {ex.Message}");
}

