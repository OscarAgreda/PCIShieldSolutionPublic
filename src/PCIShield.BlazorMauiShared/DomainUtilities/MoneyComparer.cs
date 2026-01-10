namespace PCIShield.Domain.ModelsDto;
public class MoneyComparer : IEqualityComparer<decimal>
{
    public bool Equals(decimal x, decimal y) => Math.Abs(x - y) < 0.001m;
    public int GetHashCode(decimal obj) => obj.GetHashCode();
}