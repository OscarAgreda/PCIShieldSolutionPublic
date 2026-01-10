namespace PCIShield.Domain.ModelsDto;
public class MerchantToComplianceOfficerAfterJustPickupChatDto
{
    public Guid ConversationId { get; set; }
    public Guid MoreConversationId { get; set; }
    public Guid ComplianceOfficerId { get; set; }
    public Guid MerchantId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ProductId { get; set; }
    public string Message { get; set; }
    public string MessageType { get; set; }
    public Guid MessageId { get; set; }
    public Guid TenantId { get; set; }
    public Guid MerchantUserId { get; set; }
    public Guid ComplianceOfficerUserId { get; set; }
    public Guid MessageTypeId { get; set; }
}