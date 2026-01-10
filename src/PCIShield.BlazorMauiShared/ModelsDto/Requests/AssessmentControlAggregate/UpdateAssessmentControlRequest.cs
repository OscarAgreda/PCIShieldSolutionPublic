using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.AssessmentControl
{
    public class UpdateAssessmentControlRequest : BaseRequest
    {
        public AssessmentControlDto AssessmentControl { get; set; }
    }

    public class UpdateAssessmentControlValidator : AbstractValidator<AssessmentControlDto>
    {
        public UpdateAssessmentControlValidator()
        {
            RuleFor(x => x.RowId)
                .NotEmpty()
                .WithMessage("Row Id is required")
                .WithErrorCode("ASSESSMENTCONTROL.ROWID_REQUIRED");

            RuleFor(x => x.AssessmentId)
                .NotNull()
                .WithMessage("Assessment Id is required")
                .WithErrorCode("ASSESSMENTCONTROL.ASSESSMENTID_REQUIRED")
                ;

            RuleFor(x => x.ControlId)
                .NotNull()
                .WithMessage("Control Id is required")
                .WithErrorCode("ASSESSMENTCONTROL.CONTROLID_REQUIRED")
                ;

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("ASSESSMENTCONTROL.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.TestResult)
                .NotEmpty()
                .WithMessage("Test Result is required")
                .WithErrorCode("ASSESSMENTCONTROL.TESTRESULT_REQUIRED")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("ASSESSMENTCONTROL.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("ASSESSMENTCONTROL.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("ASSESSMENTCONTROL.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("ASSESSMENTCONTROL.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("ASSESSMENTCONTROL.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.AssessmentId)
                .NotEmpty()
                .WithMessage("Assessment reference is required")
                .WithErrorCode("ASSESSMENTCONTROL.ASSESSMENT_REQUIRED");
            RuleFor(x => x.ControlId)
                .NotEmpty()
                .WithMessage("Control reference is required")
                .WithErrorCode("ASSESSMENTCONTROL.CONTROL_REQUIRED");

        }
    }
}
