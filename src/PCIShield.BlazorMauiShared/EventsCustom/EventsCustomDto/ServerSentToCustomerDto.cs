using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto
{
    public class ServerSentToMerchantDto
    {
        public Guid MerchantId { get; set; }
        public string Message { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
    }
}