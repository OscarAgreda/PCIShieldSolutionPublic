namespace PCIShield.Domain.ModelsDto;
public class ResponseToInitMerchantChatDto
{
    public Guid MessageId { get; set; }
    public string Message { get; set; }
    public Guid UnansweredConversationId { get; set; }
    public Guid UserId { get; set; }
    public Guid MessageTypeId { get; set; }
    public Guid MerchantId { get; set; }
}