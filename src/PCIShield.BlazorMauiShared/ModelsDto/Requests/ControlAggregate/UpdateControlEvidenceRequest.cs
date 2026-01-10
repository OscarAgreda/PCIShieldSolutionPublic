using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.ControlEvidence
{
    public class UpdateControlEvidenceRequest : BaseRequest
    {
        public ControlEvidenceDto ControlEvidence { get; set; }
    }

    public class UpdateControlEvidenceValidator : AbstractValidator<ControlEvidenceDto>
    {
        public UpdateControlEvidenceValidator()
        {
            RuleFor(x => x.RowId)
                .NotEmpty()
                .WithMessage("Row Id is required")
                .WithErrorCode("CONTROLEVIDENCE.ROWID_REQUIRED");

            RuleFor(x => x.ControlId)
                .NotNull()
                .WithMessage("Control Id is required")
                .WithErrorCode("CONTROLEVIDENCE.CONTROLID_REQUIRED")
                ;

            RuleFor(x => x.EvidenceId)
                .NotNull()
                .WithMessage("Evidence Id is required")
                .WithErrorCode("CONTROLEVIDENCE.EVIDENCEID_REQUIRED")
                ;

            RuleFor(x => x.AssessmentId)
                .NotNull()
                .WithMessage("Assessment Id is required")
                .WithErrorCode("CONTROLEVIDENCE.ASSESSMENTID_REQUIRED")
                ;

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("CONTROLEVIDENCE.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.IsPrimary)
                .NotNull()
                .WithMessage("Is Primary is required")
                .WithErrorCode("CONTROLEVIDENCE.ISPRIMARY_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsPrimary transition is not allowed")
                .WithErrorCode("CONTROLEVIDENCE.ISPRIMARY_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsPrimary due to business rules")
                .WithErrorCode("CONTROLEVIDENCE.ISPRIMARY_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("CONTROLEVIDENCE.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("CONTROLEVIDENCE.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("CONTROLEVIDENCE.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("CONTROLEVIDENCE.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("CONTROLEVIDENCE.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.AssessmentId)
                .NotEmpty()
                .WithMessage("Assessment reference is required")
                .WithErrorCode("CONTROLEVIDENCE.ASSESSMENT_REQUIRED");
            RuleFor(x => x.ControlId)
                .NotEmpty()
                .WithMessage("Control reference is required")
                .WithErrorCode("CONTROLEVIDENCE.CONTROL_REQUIRED");
            RuleFor(x => x.EvidenceId)
                .NotEmpty()
                .WithMessage("Evidence reference is required")
                .WithErrorCode("CONTROLEVIDENCE.EVIDENCE_REQUIRED");

        }
    }
}
