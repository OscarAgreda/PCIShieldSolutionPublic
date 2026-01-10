using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto;
public class ComplianceOfficerSendTemplateDocumentMessage
{
    public Guid ComplianceOfficerId { get; set; }
    public string Message { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
}