using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.ScanSchedule
{
    public class UpdateScanScheduleRequest : BaseRequest
    {
        public ScanScheduleDto ScanSchedule { get; set; }
    }

    public class UpdateScanScheduleValidator : AbstractValidator<ScanScheduleDto>
    {
        public UpdateScanScheduleValidator()
        {
            RuleFor(x => x.ScanScheduleId)
                .NotEmpty()
                .WithMessage("Scan Schedule Id is required")
                .WithErrorCode("SCANSCHEDULE.SCANSCHEDULEID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("SCANSCHEDULE.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.AssetId)
                .NotNull()
                .WithMessage("Asset Id is required")
                .WithErrorCode("SCANSCHEDULE.ASSETID_REQUIRED")
                ;

            RuleFor(x => x.ScanType)
                .NotEmpty()
                .WithMessage("Scan Type is required")
                .WithErrorCode("SCANSCHEDULE.SCANTYPE_REQUIRED")
                ;

            RuleFor(x => x.Frequency)
                .NotEmpty()
                .WithMessage("Frequency is required")
                .WithErrorCode("SCANSCHEDULE.FREQUENCY_REQUIRED")
                .MaximumLength(50)
                .WithMessage("Frequency cannot exceed 50 characters")
                .WithErrorCode("SCANSCHEDULE.FREQUENCY_LENGTH")
                ;

            RuleFor(x => x.NextScanDate)
                .NotNull()
                .WithMessage("Next Scan Date is required")
                .WithErrorCode("SCANSCHEDULE.NEXTSCANDATE_REQUIRED")
                ;

            RuleFor(x => x.IsEnabled)
                .NotNull()
                .WithMessage("Is Enabled is required")
                .WithErrorCode("SCANSCHEDULE.ISENABLED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsEnabled transition is not allowed")
                .WithErrorCode("SCANSCHEDULE.ISENABLED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsEnabled due to business rules")
                .WithErrorCode("SCANSCHEDULE.ISENABLED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("SCANSCHEDULE.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("SCANSCHEDULE.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("SCANSCHEDULE.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("SCANSCHEDULE.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("SCANSCHEDULE.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.AssetId)
                .NotEmpty()
                .WithMessage("Asset reference is required")
                .WithErrorCode("SCANSCHEDULE.ASSET_REQUIRED");

        }
    }
}
