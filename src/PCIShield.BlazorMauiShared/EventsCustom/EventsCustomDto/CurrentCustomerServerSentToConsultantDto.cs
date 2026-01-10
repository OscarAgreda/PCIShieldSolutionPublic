using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ardalis.GuardClauses;
using PCIShieldLib.SharedKernel.Interfaces;
namespace PCIShield.Domain.ModelsDto
{
    public class CurrentMerchantServerSentToComplianceOfficerDto
    {
        public Guid MerchantId { get;  set; }
        public string MerchantFirstName { get;  set; }
        public string MerchantLastName { get;  set; }
        public string MerchantTitle { get;  set; }
        public string MerchantJsonResume { get;  set; }
        public bool IsNaturalPerson { get;  set; }
        public Guid TenantId { get;  set; }
        public CurrentMerchantServerSentToComplianceOfficerDto() {}
    }
}
