using PCIShield.Domain.ModelsDto;
namespace PCIShield.BlazorMauiShared.CustomDto
{
    public class SearchCacheInvoiceEntry
    {
        private const int CACHE_DURATION_MINUTES = 5;
        public DateTime Timestamp { get; set; }
        public bool IsExpired => DateTime.UtcNow - Timestamp > TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
    }
    public class SearchCacheMerchantEntry
    {
        private const int CACHE_DURATION_MINUTES = 5;
        public DateTime Timestamp { get; set; }
        public bool IsExpired => DateTime.UtcNow - Timestamp > TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
    }
    public class SearchCacheSupplierEntry
    {
        private const int CACHE_DURATION_MINUTES = 5;
        public DateTime Timestamp { get; set; }
        public bool IsExpired => DateTime.UtcNow - Timestamp > TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
    }
}
