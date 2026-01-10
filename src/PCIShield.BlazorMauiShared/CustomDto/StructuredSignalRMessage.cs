namespace BlazorMauiShared.Models.Merchant
{
    public class StructuredSignalRMessage
    {
        public Guid ComplianceOfficerId { get; set; }
        public string Content { get; set; }
        public Guid MerchantId { get; set; }
        public Guid MessageId { get; set; }
        public string MessageType { get; set; }
        public Guid MessageTypeId { get; set; }
        public Guid ReceivedComplianceOfficerUserId { get; set; }
        public Guid ReceivedCategoryId { get; set; }
        public Guid ReceivedConversationId { get; set; }
        public Guid ReceivedMerchantUserId { get; set; }
        public Guid ReceivedMessageId { get; set; }
        public Guid ReceivedMoreConversationId { get; set; }
        public bool ResponseToQuestion { get; set; }
        public Guid SyntheticScopeId { get; set; }
        public Guid UnansweredConversationId { get; set; }

        public Guid ReplyToMessageId { get; set; }
        public bool IsReplyToMessage { get; set; }
        public string ReplyToMessageContent { get; set; }

        public decimal ConversationSumSpent { get; set; }
        public decimal MerchantBalance { get; set; }
        public decimal? ComplianceOfficerUnpaidBalance { get; set; }
        public decimal? ComplianceOfficerConversationCommission { get; set; }

        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsImage { get; set; }
        public bool IsVideo { get; set; }

        public bool IsPdf { get; set; }
        public bool IsMsWord { get; set; }
        public bool IsExcel { get; set; }
        public bool IsVoiceNote { get; set; }
        public bool IsText { get; set; }
    }
}