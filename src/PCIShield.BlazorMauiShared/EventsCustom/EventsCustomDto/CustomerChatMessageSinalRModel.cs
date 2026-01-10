using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto
{
    public class MerchantChatMessageSinalRModel
    {
        public Guid ComplianceOfficerId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid MerchantId { get; set; }
        public string Message { get; set; }
        public Guid MessageId { get; set; }
        public string MessageType { get; set; }
        public Guid MessageTypeId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ReceivedComplianceOfficerUserId { get; set; }
        public Guid ReceivedConversationId { get; set; }
        public Guid ReceivedMerchantUserId { get; set; }
        public Guid ReceivedMoreConversationId { get; set; }
        public Guid TenantId { get; set; }
        public string token { get; set; }
        public Guid UnansweredConversationId { get; set; }
        public string PCIShieldAppPowerUserId { get; set; }
        public Guid ReplyToMessageId { get; set; }
        public bool IsReplyToMessage { get; set; }
        public string ReplyToMessageContent { get; set; }
        public Guid CallBackFirstCheckMessageId { get; set; }
        public Guid CallBackSecondCheckMessageId { get; set; }
        public decimal ConversationSumSpent { get; set; }
        public decimal MerchantBalance { get; set; }
        public decimal? ComplianceOfficerUnpaidBalance { get; set; }
        public decimal? ComplianceOfficerConversationCommission { get; set; }
        public bool HasAttachments { get; set; }
        public bool IsChat { get; set; }
        public Guid AttachmentTypeId { get; set; }
        public decimal? AttachmentSize { get; set; }
        public string? AttachmentIconUrl { get; set; }
        public string? AttachmentPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class DarkMatterChatConversationRequest
    {
        public Guid ComplianceOfficerId { get; set; }
        public Guid MerchantId { get; set; }
        public string Message { get; set; }
        public Guid ReceivedComplianceOfficerUserId { get; set; }
        public Guid UnansweredConversationId { get; set; }
    }
}