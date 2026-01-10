using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.AssetControl
{
    public class UpdateAssetControlRequest : BaseRequest
    {
        public AssetControlDto AssetControl { get; set; }
    }

    public class UpdateAssetControlValidator : AbstractValidator<AssetControlDto>
    {
        public UpdateAssetControlValidator()
        {
            RuleFor(x => x.RowId)
                .NotEmpty()
                .WithMessage("Row Id is required")
                .WithErrorCode("ASSETCONTROL.ROWID_REQUIRED");

            RuleFor(x => x.AssetId)
                .NotNull()
                .WithMessage("Asset Id is required")
                .WithErrorCode("ASSETCONTROL.ASSETID_REQUIRED")
                ;

            RuleFor(x => x.ControlId)
                .NotNull()
                .WithMessage("Control Id is required")
                .WithErrorCode("ASSETCONTROL.CONTROLID_REQUIRED")
                ;

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("ASSETCONTROL.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.IsApplicable)
                .NotNull()
                .WithMessage("Is Applicable is required")
                .WithErrorCode("ASSETCONTROL.ISAPPLICABLE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsApplicable transition is not allowed")
                .WithErrorCode("ASSETCONTROL.ISAPPLICABLE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsApplicable due to business rules")
                .WithErrorCode("ASSETCONTROL.ISAPPLICABLE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("ASSETCONTROL.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("ASSETCONTROL.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("ASSETCONTROL.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("ASSETCONTROL.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("ASSETCONTROL.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.AssetId)
                .NotEmpty()
                .WithMessage("Asset reference is required")
                .WithErrorCode("ASSETCONTROL.ASSET_REQUIRED");
            RuleFor(x => x.ControlId)
                .NotEmpty()
                .WithMessage("Control reference is required")
                .WithErrorCode("ASSETCONTROL.CONTROL_REQUIRED");

        }
    }
}
