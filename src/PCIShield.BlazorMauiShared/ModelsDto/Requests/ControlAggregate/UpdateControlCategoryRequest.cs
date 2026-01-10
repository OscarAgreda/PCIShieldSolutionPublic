using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.ControlCategory
{
    public class UpdateControlCategoryRequest : BaseRequest
    {
        public ControlCategoryDto ControlCategory { get; set; }
    }

    public class UpdateControlCategoryValidator : AbstractValidator<ControlCategoryDto>
    {
        public UpdateControlCategoryValidator()
        {
            RuleFor(x => x.ControlCategoryId)
                .NotEmpty()
                .WithMessage("Control Category Id is required")
                .WithErrorCode("CONTROLCATEGORY.CONTROLCATEGORYID_REQUIRED");

            RuleFor(x => x.ControlCategoryCode)
                .NotEmpty()
                .WithMessage("Control Category Code is required")
                .WithErrorCode("CONTROLCATEGORY.CONTROLCATEGORYCODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("ControlCategoryCode cannot exceed 30 characters")
                .WithErrorCode("CONTROLCATEGORY.CONTROLCATEGORYCODE_LENGTH")
                ;

            RuleFor(x => x.ControlCategoryName)
                .NotEmpty()
                .WithMessage("Control Category Name is required")
                .WithErrorCode("CONTROLCATEGORY.CONTROLCATEGORYNAME_REQUIRED")
                .MaximumLength(100)
                .WithMessage("ControlCategoryName cannot exceed 100 characters")
                .WithErrorCode("CONTROLCATEGORY.CONTROLCATEGORYNAME_LENGTH")
                ;

            RuleFor(x => x.RequirementSection)
                .NotEmpty()
                .WithMessage("Requirement Section is required")
                .WithErrorCode("CONTROLCATEGORY.REQUIREMENTSECTION_REQUIRED")
                .MaximumLength(20)
                .WithMessage("RequirementSection cannot exceed 20 characters")
                .WithErrorCode("CONTROLCATEGORY.REQUIREMENTSECTION_LENGTH")
                ;

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Is Active is required")
                .WithErrorCode("CONTROLCATEGORY.ISACTIVE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsActive transition is not allowed")
                .WithErrorCode("CONTROLCATEGORY.ISACTIVE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsActive due to business rules")
                .WithErrorCode("CONTROLCATEGORY.ISACTIVE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("CONTROLCATEGORY.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("CONTROLCATEGORY.CREATEDBY_REQUIRED")
                ;

        }
    }
}
