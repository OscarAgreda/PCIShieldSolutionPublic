using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
namespace PCIShield.Domain.ModelsDto
{
    public class AvailableComplianceOfficerServerSentToMerchantDto
    {
        public string ComplianceOfficerFirstName { get; set; }
        public Guid ComplianceOfficerId { get; set; }
        public string ComplianceOfficerLastName { get; set; }
        public string ComplianceOfficerTitle { get; set; }
        public Guid? ComplianceOfficerUserId { get; set; }
        public string ComplianceOfficerCode { get; set; }
    }
}