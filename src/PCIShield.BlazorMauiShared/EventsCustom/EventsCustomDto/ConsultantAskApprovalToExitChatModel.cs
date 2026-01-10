using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto;
public class ComplianceOfficerAskApprovalToExitChatModel
{
    [JsonConstructor]
    public ComplianceOfficerAskApprovalToExitChatModel()
    {
    }
    public ComplianceOfficerAskApprovalToExitChatModel(bool complianceOfficerWasApprovedToExit,string messageText, Guid messageId, Guid conversationId, Guid complianceOfficerId, Guid merchantId, Guid complianceOfficerUserId, Guid merchantUserId)
    {
        ComplianceOfficerWasApprovedToExit = Guard.Against.Null(complianceOfficerWasApprovedToExit, nameof(complianceOfficerWasApprovedToExit));
        MessageText = Guard.Against.NullOrEmpty(messageText, nameof(messageText));
        MessageId = Guard.Against.NullOrEmpty(messageId, nameof(messageId));
        ConversationId = Guard.Against.NullOrEmpty(conversationId, nameof(conversationId));
        ComplianceOfficerId = Guard.Against.NullOrEmpty(complianceOfficerId, nameof(complianceOfficerId));
        MerchantId = Guard.Against.NullOrEmpty(merchantId, nameof(merchantId));
        ComplianceOfficerUserId = Guard.Against.NullOrEmpty(complianceOfficerUserId, nameof(complianceOfficerUserId));
        MerchantUserId = Guard.Against.NullOrEmpty(merchantUserId, nameof(merchantUserId));
    }
    public bool ComplianceOfficerWasApprovedToExit { get; set; }
    public string MessageText { get; set; }
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid ComplianceOfficerId { get; set; }
    public Guid MerchantId { get; set; }
    public Guid ComplianceOfficerUserId { get; set; }
    public Guid MerchantUserId { get; set; }
}