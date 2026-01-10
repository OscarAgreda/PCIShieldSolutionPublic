using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto;
public class MerchantSentVideoMessage
{
    public Guid MerchantId { get; set; }
    public Guid MerchantJunctionCategoryId { get; set; }
    public string Message { get; set; }
    public Guid MessageId { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
}