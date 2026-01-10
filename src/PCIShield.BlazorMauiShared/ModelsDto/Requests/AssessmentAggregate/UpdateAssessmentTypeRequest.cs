using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.AssessmentType
{
    public class UpdateAssessmentTypeRequest : BaseRequest
    {
        public AssessmentTypeDto AssessmentType { get; set; }
    }

    public class UpdateAssessmentTypeValidator : AbstractValidator<AssessmentTypeDto>
    {
        public UpdateAssessmentTypeValidator()
        {
            RuleFor(x => x.AssessmentTypeId)
                .NotEmpty()
                .WithMessage("Assessment Type Id is required")
                .WithErrorCode("ASSESSMENTTYPE.ASSESSMENTTYPEID_REQUIRED");

            RuleFor(x => x.AssessmentTypeCode)
                .NotEmpty()
                .WithMessage("Assessment Type Code is required")
                .WithErrorCode("ASSESSMENTTYPE.ASSESSMENTTYPECODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("AssessmentTypeCode cannot exceed 30 characters")
                .WithErrorCode("ASSESSMENTTYPE.ASSESSMENTTYPECODE_LENGTH")
                ;

            RuleFor(x => x.AssessmentTypeName)
                .NotEmpty()
                .WithMessage("Assessment Type Name is required")
                .WithErrorCode("ASSESSMENTTYPE.ASSESSMENTTYPENAME_REQUIRED")
                .MaximumLength(100)
                .WithMessage("AssessmentTypeName cannot exceed 100 characters")
                .WithErrorCode("ASSESSMENTTYPE.ASSESSMENTTYPENAME_LENGTH")
                ;

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters")
                .WithErrorCode("ASSESSMENTTYPE.DESCRIPTION_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("Description has an invalid format")
                .WithErrorCode("ASSESSMENTTYPE.DESCRIPTION_FORMAT")
                ;

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Is Active is required")
                .WithErrorCode("ASSESSMENTTYPE.ISACTIVE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsActive transition is not allowed")
                .WithErrorCode("ASSESSMENTTYPE.ISACTIVE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsActive due to business rules")
                .WithErrorCode("ASSESSMENTTYPE.ISACTIVE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("ASSESSMENTTYPE.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("ASSESSMENTTYPE.CREATEDBY_REQUIRED")
                ;

        }
    }
}
