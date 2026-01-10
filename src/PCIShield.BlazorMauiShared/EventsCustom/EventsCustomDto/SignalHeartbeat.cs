using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto;
public class SignalHeartbeat
{
    public SignalHeartbeat(DateTime mauiTime, Guid userId)
    {
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
        MauiTime = Guard.Against.OutOfSQLDateRange(mauiTime, nameof(mauiTime));
    }
    public Guid UserId { get; set; }
    public DateTime MauiTime { get; set; }
}