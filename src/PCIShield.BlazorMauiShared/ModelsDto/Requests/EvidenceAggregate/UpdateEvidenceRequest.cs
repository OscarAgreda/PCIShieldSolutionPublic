using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.Evidence
{
    public class UpdateEvidenceRequest : BaseRequest
    {
        public EvidenceDto Evidence { get; set; }
    }

    public class UpdateEvidenceValidator : AbstractValidator<EvidenceDto>
    {
        public UpdateEvidenceValidator()
        {
            RuleFor(x => x.EvidenceId)
                .NotEmpty()
                .WithMessage("Evidence Id is required")
                .WithErrorCode("EVIDENCE.EVIDENCEID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("EVIDENCE.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("EVIDENCE.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.EvidenceCode)
                .NotEmpty()
                .WithMessage("Evidence Code is required")
                .WithErrorCode("EVIDENCE.EVIDENCECODE_REQUIRED")
                .MaximumLength(50)
                .WithMessage("EvidenceCode cannot exceed 50 characters")
                .WithErrorCode("EVIDENCE.EVIDENCECODE_LENGTH")
                ;

            RuleFor(x => x.EvidenceTitle)
                .NotEmpty()
                .WithMessage("Evidence Title is required")
                .WithErrorCode("EVIDENCE.EVIDENCETITLE_REQUIRED")
                .MaximumLength(200)
                .WithMessage("EvidenceTitle cannot exceed 200 characters")
                .WithErrorCode("EVIDENCE.EVIDENCETITLE_LENGTH")
                ;

            RuleFor(x => x.EvidenceType)
                .NotEmpty()
                .WithMessage("Evidence Type is required")
                .WithErrorCode("EVIDENCE.EVIDENCETYPE_REQUIRED")
                ;

            RuleFor(x => x.CollectedDate)
                .NotNull()
                .WithMessage("Collected Date is required")
                .WithErrorCode("EVIDENCE.COLLECTEDDATE_REQUIRED")
                ;

            RuleFor(x => x.FileHash)
                .MaximumLength(128)
                .WithMessage("FileHash cannot exceed 128 characters")
                .WithErrorCode("EVIDENCE.FILEHASH_LENGTH")
                .Must(password => {
                        if (string.IsNullOrEmpty(password)) return true;
                        var hasNumber = new System.Text.RegularExpressions.Regex(@"[0-9]+");
                        var hasUpperChar = new System.Text.RegularExpressions.Regex(@"[A-Z]+");
                        var hasLowerChar = new System.Text.RegularExpressions.Regex(@"[a-z]+");
                        var hasSymbols = new System.Text.RegularExpressions.Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
                        
                        return password.Length >= 8 &&
                               hasNumber.IsMatch(password) &&
                               hasUpperChar.IsMatch(password) &&
                               hasLowerChar.IsMatch(password) &&
                               hasSymbols.IsMatch(password);
                    })
                .WithMessage("Password must be at least 8 characters and contain uppercase, lowercase, number, and special characters")
                .WithErrorCode("EVIDENCE.FILEHASH_PASSWORD_STRENGTH_INVALID")
                ;

            RuleFor(x => x.StorageUri)
                .MaximumLength(500)
                .WithMessage("StorageUri cannot exceed 500 characters")
                .WithErrorCode("EVIDENCE.STORAGEURI_LENGTH")
                ;

            RuleFor(x => x.IsValid)
                .NotNull()
                .WithMessage("Is Valid is required")
                .WithErrorCode("EVIDENCE.ISVALID_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsValid transition is not allowed")
                .WithErrorCode("EVIDENCE.ISVALID_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsValid due to business rules")
                .WithErrorCode("EVIDENCE.ISVALID_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("EVIDENCE.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("EVIDENCE.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("EVIDENCE.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("EVIDENCE.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("EVIDENCE.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.ControlEvidences)
                .Must(items => items == null || items.Count(i => i.IsPrimary) == 1)
                .When(x => x.ControlEvidences != null && x.ControlEvidences.Any())
                .WithMessage("Must have exactly one primary ControlEvidence")
                .WithErrorCode("CONTROLEVIDENCE_PRIMARY_REQUIRED");

            RuleForEach(x => x.ControlEvidences )
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.EvidenceId)
                        .NotEmpty()
                        .WithMessage("Evidence reference is required")
                        .WithErrorCode("CONTROLEVIDENCE_LEFT_REQUIRED");
                    item.RuleFor(x => x.ControlId)
                        .NotEmpty()
                        .WithMessage("Control reference is required")
                        .WithErrorCode("CONTROLEVIDENCE_RIGHT_REQUIRED");
                    item.RuleFor(x => x.TenantId)
                        .NotNull()
                        .WithMessage("TenantId is required")
                        .WithErrorCode("CONTROLEVIDENCE.TENANTID_REQUIRED_INVALID");
                    item.RuleFor(x => x.IsPrimary)
                        .NotNull()
                        .WithMessage("IsPrimary is required")
                        .WithErrorCode("CONTROLEVIDENCE.ISPRIMARY_REQUIRED_INVALID");
                    item.RuleFor(x => x.CreatedAt)
                        .NotNull()
                        .WithMessage("CreatedAt is required")
                        .WithErrorCode("CONTROLEVIDENCE.CREATEDAT_REQUIRED_INVALID");
                    item.RuleFor(x => x.CreatedBy)
                        .NotNull()
                        .WithMessage("CreatedBy is required")
                        .WithErrorCode("CONTROLEVIDENCE.CREATEDBY_REQUIRED_INVALID");
                    item.RuleFor(x => x.IsDeleted)
                        .NotNull()
                        .WithMessage("IsDeleted is required")
                        .WithErrorCode("CONTROLEVIDENCE.ISDELETED_REQUIRED_INVALID");
                });

            RuleFor(x => x.ControlEvidences )
                .Must(items => items == null || !Vali.HasDuplicateJoinEntries(items))
                .WithMessage("Duplicate ControlEvidence entries are not allowed")
                .WithErrorCode("CONTROLEVIDENCE_DUPLICATE");
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("EVIDENCE.MERCHANT_REQUIRED");

        }
    }
}
