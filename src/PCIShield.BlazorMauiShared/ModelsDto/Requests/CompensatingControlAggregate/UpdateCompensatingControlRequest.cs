using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.CompensatingControl
{
    public class UpdateCompensatingControlRequest : BaseRequest
    {
        public CompensatingControlDto CompensatingControl { get; set; }
    }

    public class UpdateCompensatingControlValidator : AbstractValidator<CompensatingControlDto>
    {
        public UpdateCompensatingControlValidator()
        {
            RuleFor(x => x.CompensatingControlId)
                .NotEmpty()
                .WithMessage("Compensating Control Id is required")
                .WithErrorCode("COMPENSATINGCONTROL.COMPENSATINGCONTROLID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("COMPENSATINGCONTROL.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.ControlId)
                .NotNull()
                .WithMessage("Control Id is required")
                .WithErrorCode("COMPENSATINGCONTROL.CONTROLID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("COMPENSATINGCONTROL.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.Justification)
                .NotEmpty()
                .WithMessage("Justification is required")
                .WithErrorCode("COMPENSATINGCONTROL.JUSTIFICATION_REQUIRED")
                ;

            RuleFor(x => x.ImplementationDetails)
                .NotEmpty()
                .WithMessage("Implementation Details is required")
                .WithErrorCode("COMPENSATINGCONTROL.IMPLEMENTATIONDETAILS_REQUIRED")
                ;

            RuleFor(x => x.ExpiryDate)
                .NotNull()
                .WithMessage("Expiry Date is required")
                .WithErrorCode("COMPENSATINGCONTROL.EXPIRYDATE_REQUIRED")
                ;

            RuleFor(x => x.Rank)
                .NotEmpty()
                .WithMessage("Rank is required")
                .WithErrorCode("COMPENSATINGCONTROL.RANK_REQUIRED")
                .InclusiveBetween(0, 100)
                .WithMessage("Rank must be between 0 and 100")
                .WithErrorCode("COMPENSATINGCONTROL.RANK_RANGE_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("COMPENSATINGCONTROL.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("COMPENSATINGCONTROL.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("COMPENSATINGCONTROL.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("COMPENSATINGCONTROL.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("COMPENSATINGCONTROL.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.ControlId)
                .NotEmpty()
                .WithMessage("Control reference is required")
                .WithErrorCode("COMPENSATINGCONTROL.CONTROL_REQUIRED");
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("COMPENSATINGCONTROL.MERCHANT_REQUIRED");

        }
    }
}
