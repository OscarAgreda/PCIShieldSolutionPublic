using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto;
public class ComplianceOfficerEndedChatDueToInactivity
{
    [JsonConstructor]
    public ComplianceOfficerEndedChatDueToInactivity()
    { }
    public ComplianceOfficerEndedChatDueToInactivity(Guid complianceOfficerId, Guid userId, Guid tenantId, string message)
    {
        ComplianceOfficerId = Guard.Against.NullOrEmpty(complianceOfficerId, nameof(complianceOfficerId));
        TenantId = Guard.Against.NullOrEmpty(tenantId, nameof(tenantId));
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
        Message = Guard.Against.NullOrWhiteSpace(message, nameof(message));
    }
    public Guid ComplianceOfficerId { get; set; }
    public string Message { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
}