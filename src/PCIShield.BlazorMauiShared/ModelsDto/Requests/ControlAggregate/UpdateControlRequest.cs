using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.CompensatingControl;

namespace PCIShield.BlazorMauiShared.Models.Control
{
    public class UpdateControlRequest : BaseRequest
    {
        public ControlDto Control { get; set; }
    }

    public class UpdateControlValidator : AbstractValidator<ControlDto>
    {
        public UpdateControlValidator()
        {
            RuleFor(x => x.ControlId)
                .NotEmpty()
                .WithMessage("Control Id is required")
                .WithErrorCode("CONTROL.CONTROLID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("CONTROL.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.ControlCode)
                .NotEmpty()
                .WithMessage("Control Code is required")
                .WithErrorCode("CONTROL.CONTROLCODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("ControlCode cannot exceed 30 characters")
                .WithErrorCode("CONTROL.CONTROLCODE_LENGTH")
                ;

            RuleFor(x => x.RequirementNumber)
                .NotEmpty()
                .WithMessage("Requirement Number is required")
                .WithErrorCode("CONTROL.REQUIREMENTNUMBER_REQUIRED")
                .MaximumLength(20)
                .WithMessage("RequirementNumber cannot exceed 20 characters")
                .WithErrorCode("CONTROL.REQUIREMENTNUMBER_LENGTH")
                ;

            RuleFor(x => x.ControlTitle)
                .NotEmpty()
                .WithMessage("Control Title is required")
                .WithErrorCode("CONTROL.CONTROLTITLE_REQUIRED")
                .MaximumLength(500)
                .WithMessage("ControlTitle cannot exceed 500 characters")
                .WithErrorCode("CONTROL.CONTROLTITLE_LENGTH")
                ;

            RuleFor(x => x.ControlDescription)
                .NotEmpty()
                .WithMessage("Control Description is required")
                .WithErrorCode("CONTROL.CONTROLDESCRIPTION_REQUIRED")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("ControlDescription has an invalid format")
                .WithErrorCode("CONTROL.CONTROLDESCRIPTION_FORMAT")
                ;

            RuleFor(x => x.FrequencyDays)
                .NotEmpty()
                .WithMessage("Frequency Days is required")
                .WithErrorCode("CONTROL.FREQUENCYDAYS_REQUIRED")
                ;

            RuleFor(x => x.IsMandatory)
                .NotNull()
                .WithMessage("Is Mandatory is required")
                .WithErrorCode("CONTROL.ISMANDATORY_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsMandatory transition is not allowed")
                .WithErrorCode("CONTROL.ISMANDATORY_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsMandatory due to business rules")
                .WithErrorCode("CONTROL.ISMANDATORY_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.EffectiveDate)
                .NotNull()
                .WithMessage("Effective Date is required")
                .WithErrorCode("CONTROL.EFFECTIVEDATE_REQUIRED")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("CONTROL.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("CONTROL.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("CONTROL.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("CONTROL.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("CONTROL.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleForEach(x => x.AssessmentControls )
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ControlId)
                        .NotEmpty()
                        .WithMessage("Control reference is required")
                        .WithErrorCode("ASSESSMENTCONTROL_LEFT_REQUIRED");
                    item.RuleFor(x => x.AssessmentId)
                        .NotEmpty()
                        .WithMessage("Assessment reference is required")
                        .WithErrorCode("ASSESSMENTCONTROL_RIGHT_REQUIRED");
                    item.RuleFor(x => x.TenantId)
                        .NotNull()
                        .WithMessage("TenantId is required")
                        .WithErrorCode("ASSESSMENTCONTROL.TENANTID_REQUIRED_INVALID");
                    item.RuleFor(x => x.TestResult)
                        .NotEmpty()
                        .WithMessage("TestResult is required")
                        .WithErrorCode("ASSESSMENTCONTROL.TESTRESULT_REQUIRED_INVALID");
                    item.RuleFor(x => x.CreatedAt)
                        .NotNull()
                        .WithMessage("CreatedAt is required")
                        .WithErrorCode("ASSESSMENTCONTROL.CREATEDAT_REQUIRED_INVALID");
                    item.RuleFor(x => x.CreatedBy)
                        .NotNull()
                        .WithMessage("CreatedBy is required")
                        .WithErrorCode("ASSESSMENTCONTROL.CREATEDBY_REQUIRED_INVALID");
                    item.RuleFor(x => x.IsDeleted)
                        .NotNull()
                        .WithMessage("IsDeleted is required")
                        .WithErrorCode("ASSESSMENTCONTROL.ISDELETED_REQUIRED_INVALID");
                });

            RuleFor(x => x.AssessmentControls )
                .Must(items => items == null || !Vali.HasDuplicateJoinEntries(items))
                .WithMessage("Duplicate AssessmentControl entries are not allowed")
                .WithErrorCode("ASSESSMENTCONTROL_DUPLICATE");
            RuleForEach(x => x.AssetControls )
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ControlId)
                        .NotEmpty()
                        .WithMessage("Control reference is required")
                        .WithErrorCode("ASSETCONTROL_LEFT_REQUIRED");
                    item.RuleFor(x => x.AssetId)
                        .NotEmpty()
                        .WithMessage("Asset reference is required")
                        .WithErrorCode("ASSETCONTROL_RIGHT_REQUIRED");
                    item.RuleFor(x => x.TenantId)
                        .NotNull()
                        .WithMessage("TenantId is required")
                        .WithErrorCode("ASSETCONTROL.TENANTID_REQUIRED_INVALID");
                    item.RuleFor(x => x.IsApplicable)
                        .NotNull()
                        .WithMessage("IsApplicable is required")
                        .WithErrorCode("ASSETCONTROL.ISAPPLICABLE_REQUIRED_INVALID");
                    item.RuleFor(x => x.CreatedAt)
                        .NotNull()
                        .WithMessage("CreatedAt is required")
                        .WithErrorCode("ASSETCONTROL.CREATEDAT_REQUIRED_INVALID");
                    item.RuleFor(x => x.CreatedBy)
                        .NotNull()
                        .WithMessage("CreatedBy is required")
                        .WithErrorCode("ASSETCONTROL.CREATEDBY_REQUIRED_INVALID");
                    item.RuleFor(x => x.IsDeleted)
                        .NotNull()
                        .WithMessage("IsDeleted is required")
                        .WithErrorCode("ASSETCONTROL.ISDELETED_REQUIRED_INVALID");
                });

            RuleFor(x => x.AssetControls )
                .Must(items => items == null || !Vali.HasDuplicateJoinEntries(items))
                .WithMessage("Duplicate AssetControl entries are not allowed")
                .WithErrorCode("ASSETCONTROL_DUPLICATE");
            RuleFor(x => x.ControlEvidences)
                .Must(items => items == null || items.Count(i => i.IsPrimary) == 1)
                .When(x => x.ControlEvidences != null && x.ControlEvidences.Any())
                .WithMessage("Must have exactly one primary ControlEvidence")
                .WithErrorCode("CONTROLEVIDENCE_PRIMARY_REQUIRED");

            RuleForEach(x => x.ControlEvidences )
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ControlId)
                        .NotEmpty()
                        .WithMessage("Control reference is required")
                        .WithErrorCode("CONTROLEVIDENCE_LEFT_REQUIRED");
                    item.RuleFor(x => x.EvidenceId)
                        .NotEmpty()
                        .WithMessage("Evidence reference is required")
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

        }
    }
}
