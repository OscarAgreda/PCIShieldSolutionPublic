namespace PCIShield.Domain.ModelsDto;
public class DateTimeComparer : IEqualityComparer<DateTime>
{
    public bool Equals(DateTime x, DateTime y) => x.ToUniversalTime() == y.ToUniversalTime();
    public int GetHashCode(DateTime obj) => obj.ToUniversalTime().GetHashCode();
}