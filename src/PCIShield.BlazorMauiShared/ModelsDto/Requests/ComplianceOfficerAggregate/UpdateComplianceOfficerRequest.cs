using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.ComplianceOfficer
{
    public class UpdateComplianceOfficerRequest : BaseRequest
    {
        public ComplianceOfficerDto ComplianceOfficer { get; set; }
    }

    public class UpdateComplianceOfficerValidator : AbstractValidator<ComplianceOfficerDto>
    {
        public UpdateComplianceOfficerValidator()
        {
            RuleFor(x => x.ComplianceOfficerId)
                .NotEmpty()
                .WithMessage("Compliance Officer Id is required")
                .WithErrorCode("COMPLIANCEOFFICER.COMPLIANCEOFFICERID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("COMPLIANCEOFFICER.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("COMPLIANCEOFFICER.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.OfficerCode)
                .NotEmpty()
                .WithMessage("Officer Code is required")
                .WithErrorCode("COMPLIANCEOFFICER.OFFICERCODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("OfficerCode cannot exceed 30 characters")
                .WithErrorCode("COMPLIANCEOFFICER.OFFICERCODE_LENGTH")
                ;

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First Name is required")
                .WithErrorCode("COMPLIANCEOFFICER.FIRSTNAME_REQUIRED")
                .MaximumLength(100)
                .WithMessage("FirstName cannot exceed 100 characters")
                .WithErrorCode("COMPLIANCEOFFICER.FIRSTNAME_LENGTH")
                ;

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last Name is required")
                .WithErrorCode("COMPLIANCEOFFICER.LASTNAME_REQUIRED")
                .MaximumLength(100)
                .WithMessage("LastName cannot exceed 100 characters")
                .WithErrorCode("COMPLIANCEOFFICER.LASTNAME_LENGTH")
                ;

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .WithErrorCode("COMPLIANCEOFFICER.EMAIL_REQUIRED")
                .MaximumLength(320)
                .WithMessage("Email cannot exceed 320 characters")
                .WithErrorCode("COMPLIANCEOFFICER.EMAIL_LENGTH")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Email has an invalid format")
                .WithErrorCode("COMPLIANCEOFFICER.EMAIL_FORMAT")
                .Must(email =>
            {
                try
                {
                    var addr = new MailAddress(email);
                    if (!string.Equals(addr.Address, email, StringComparison.Ordinal)) return false;
                    var parts = email.Split('@');
                    if (parts.Length != 2) return false;

                    var local = parts[0];
                    var domain = parts[1];
                    if (local.Length == 0 || local.Length > 64) return false;
                    if (email.Length > 254) return false;
                    if (!domain.Contains('.')) return false;

                    return true;
                }
                catch
                {
                    return false;
                }
            })
                .WithMessage("Email must be a valid email address")
                .WithErrorCode("COMPLIANCEOFFICER.EMAIL_EMAIL_INVALID")
                ;

            RuleFor(x => x.Phone)
                .MaximumLength(32)
                .WithMessage("Phone cannot exceed 32 characters")
                .WithErrorCode("COMPLIANCEOFFICER.PHONE_LENGTH")
                .Must(phone => {
                if (string.IsNullOrEmpty(phone)) return true;
                return System.Text.RegularExpressions.Regex.IsMatch(
                    phone,
                    @"^\+?[1-9]\d{1,14}$"
                );
            })
                .WithMessage("Phone must be a valid international phone number")
                .WithErrorCode("COMPLIANCEOFFICER.PHONE_PHONE_INVALID")
                ;

            RuleFor(x => x.CertificationLevel)
                .MaximumLength(50)
                .WithMessage("CertificationLevel cannot exceed 50 characters")
                .WithErrorCode("COMPLIANCEOFFICER.CERTIFICATIONLEVEL_LENGTH")
                ;

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Is Active is required")
                .WithErrorCode("COMPLIANCEOFFICER.ISACTIVE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsActive transition is not allowed")
                .WithErrorCode("COMPLIANCEOFFICER.ISACTIVE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsActive due to business rules")
                .WithErrorCode("COMPLIANCEOFFICER.ISACTIVE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("COMPLIANCEOFFICER.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("COMPLIANCEOFFICER.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("COMPLIANCEOFFICER.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("COMPLIANCEOFFICER.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("COMPLIANCEOFFICER.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("COMPLIANCEOFFICER.MERCHANT_REQUIRED");
            RuleFor(x => x.Merchant)
                .Must(parent => Vali.ValidateParentStatus(parent))
                .When(x => x.IsActive)
                .WithMessage("Merchant must be in valid status")
                .WithErrorCode("COMPLIANCEOFFICER.MERCHANT_STATUS");
        }
    }
}
