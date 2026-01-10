using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.ROCPackage;

namespace PCIShield.BlazorMauiShared.Models.Assessment
{
    public class UpdateAssessmentRequest : BaseRequest
    {
        public AssessmentDto Assessment { get; set; }
    }

    public class UpdateAssessmentValidator : AbstractValidator<AssessmentDto>
    {
        public UpdateAssessmentValidator()
        {
            RuleFor(x => x.AssessmentId)
                .NotEmpty()
                .WithMessage("Assessment Id is required")
                .WithErrorCode("ASSESSMENT.ASSESSMENTID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("ASSESSMENT.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("ASSESSMENT.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.AssessmentCode)
                .NotEmpty()
                .WithMessage("Assessment Code is required")
                .WithErrorCode("ASSESSMENT.ASSESSMENTCODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("AssessmentCode cannot exceed 30 characters")
                .WithErrorCode("ASSESSMENT.ASSESSMENTCODE_LENGTH")
                ;

            RuleFor(x => x.AssessmentType)
                .NotEmpty()
                .WithMessage("Assessment Type is required")
                .WithErrorCode("ASSESSMENT.ASSESSMENTTYPE_REQUIRED")
                ;

            RuleFor(x => x.AssessmentPeriod)
                .NotEmpty()
                .WithMessage("Assessment Period is required")
                .WithErrorCode("ASSESSMENT.ASSESSMENTPERIOD_REQUIRED")
                .MaximumLength(20)
                .WithMessage("AssessmentPeriod cannot exceed 20 characters")
                .WithErrorCode("ASSESSMENT.ASSESSMENTPERIOD_LENGTH")
                ;

            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage("Start Date is required")
                .WithErrorCode("ASSESSMENT.STARTDATE_REQUIRED")
                ;

            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage("End Date is required")
                .WithErrorCode("ASSESSMENT.ENDDATE_REQUIRED")
                ;

            RuleFor(x => x.Rank)
                .NotEmpty()
                .WithMessage("Rank is required")
                .WithErrorCode("ASSESSMENT.RANK_REQUIRED")
                .InclusiveBetween(0, 100)
                .WithMessage("Rank must be between 0 and 100")
                .WithErrorCode("ASSESSMENT.RANK_RANGE_INVALID")
                ;

            RuleFor(x => x.ComplianceScore)
                .InclusiveBetween(0M, 5M)
                .WithMessage("ComplianceScore must be between 0 and 5")
                .WithErrorCode("ASSESSMENT.COMPLIANCESCORE_RANGE_INVALID")
                ;

            RuleFor(x => x.QSAReviewRequired)
                .NotNull()
                .WithMessage("Qsareview Required is required")
                .WithErrorCode("ASSESSMENT.QSAREVIEWREQUIRED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("QSAReviewRequired transition is not allowed")
                .WithErrorCode("ASSESSMENT.QSAREVIEWREQUIRED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change QSAReviewRequired due to business rules")
                .WithErrorCode("ASSESSMENT.QSAREVIEWREQUIRED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("ASSESSMENT.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("ASSESSMENT.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("ASSESSMENT.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("ASSESSMENT.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("ASSESSMENT.ISDELETED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be after start date")
                .WithErrorCode("ASSESSMENT_INVALID_DATE_RANGE");
            RuleForEach(x => x.AssessmentControls )
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.AssessmentId)
                        .NotEmpty()
                        .WithMessage("Assessment reference is required")
                        .WithErrorCode("ASSESSMENTCONTROL_LEFT_REQUIRED");
                    item.RuleFor(x => x.ControlId)
                        .NotEmpty()
                        .WithMessage("Control reference is required")
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
            RuleFor(x => x.ControlEvidences)
                .Must(items => items == null || items.Count(i => i.IsPrimary) == 1)
                .When(x => x.ControlEvidences != null && x.ControlEvidences.Any())
                .WithMessage("Must have exactly one primary ControlEvidence")
                .WithErrorCode("CONTROLEVIDENCE_PRIMARY_REQUIRED");

            RuleForEach(x => x.ControlEvidences )
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.AssessmentId)
                        .NotEmpty()
                        .WithMessage("Assessment reference is required")
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
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("ASSESSMENT.MERCHANT_REQUIRED");

        }
    }
}
