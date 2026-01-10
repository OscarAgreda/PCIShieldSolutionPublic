using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto
{
    public class ComplianceOfficerInitChatSentToServerDto
    {
        [JsonConstructor]
        public ComplianceOfficerInitChatSentToServerDto()
        { }
        public ComplianceOfficerInitChatSentToServerDto(Guid complianceOfficerJunctionCategoryId, Guid complianceOfficerId,   Guid messageId, Guid userId, Guid tenantId, string message)
        {
            ComplianceOfficerJunctionCategoryId = Guard.Against.NullOrEmpty(complianceOfficerJunctionCategoryId, nameof(complianceOfficerJunctionCategoryId));
            ComplianceOfficerId = Guard.Against.NullOrEmpty(complianceOfficerId, nameof(complianceOfficerId));
            MessageId = Guard.Against.NullOrEmpty(messageId, nameof(messageId));
            TenantId = Guard.Against.NullOrEmpty(tenantId, nameof(tenantId));
            UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
            Message = Guard.Against.NullOrWhiteSpace(message, nameof(message));
        }
        public Guid ComplianceOfficerId { get; set; }
        public Guid ComplianceOfficerJunctionCategoryId { get; set; }
        public string Message { get; set; }
        public Guid MessageId { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
    }
}