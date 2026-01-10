using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.ScanSchedule;
using PCIShield.BlazorMauiShared.Models.Vulnerability;

namespace PCIShield.BlazorMauiShared.Models.Asset
{
    public class UpdateAssetRequest : BaseRequest
    {
        public AssetDto Asset { get; set; }
    }

    public class UpdateAssetValidator : AbstractValidator<AssetDto>
    {
        public UpdateAssetValidator()
        {
            RuleFor(x => x.AssetId)
                .NotEmpty()
                .WithMessage("Asset Id is required")
                .WithErrorCode("ASSET.ASSETID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("ASSET.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("ASSET.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.AssetCode)
                .NotEmpty()
                .WithMessage("Asset Code is required")
                .WithErrorCode("ASSET.ASSETCODE_REQUIRED")
                .MaximumLength(50)
                .WithMessage("AssetCode cannot exceed 50 characters")
                .WithErrorCode("ASSET.ASSETCODE_LENGTH")
                ;

            RuleFor(x => x.AssetName)
                .NotEmpty()
                .WithMessage("Asset Name is required")
                .WithErrorCode("ASSET.ASSETNAME_REQUIRED")
                .MaximumLength(200)
                .WithMessage("AssetName cannot exceed 200 characters")
                .WithErrorCode("ASSET.ASSETNAME_LENGTH")
                ;

            RuleFor(x => x.AssetType)
                .NotEmpty()
                .WithMessage("Asset Type is required")
                .WithErrorCode("ASSET.ASSETTYPE_REQUIRED")
                ;

            RuleFor(x => x.IPAddress)
                .MaximumLength(45)
                .WithMessage("IPAddress cannot exceed 45 characters")
                .WithErrorCode("ASSET.IPADDRESS_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("IPAddress has an invalid format")
                .WithErrorCode("ASSET.IPADDRESS_FORMAT")
                ;

            RuleFor(x => x.Hostname)
                .MaximumLength(255)
                .WithMessage("Hostname cannot exceed 255 characters")
                .WithErrorCode("ASSET.HOSTNAME_LENGTH")
                ;

            RuleFor(x => x.IsInCDE)
                .NotNull()
                .WithMessage("Is In CDE is required")
                .WithErrorCode("ASSET.ISINCDE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsInCDE transition is not allowed")
                .WithErrorCode("ASSET.ISINCDE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsInCDE due to business rules")
                .WithErrorCode("ASSET.ISINCDE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.NetworkZone)
                .MaximumLength(100)
                .WithMessage("NetworkZone cannot exceed 100 characters")
                .WithErrorCode("ASSET.NETWORKZONE_LENGTH")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("ASSET.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("ASSET.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("ASSET.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("ASSET.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("ASSET.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleForEach(x => x.AssetControls )
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.AssetId)
                        .NotEmpty()
                        .WithMessage("Asset reference is required")
                        .WithErrorCode("ASSETCONTROL_LEFT_REQUIRED");
                    item.RuleFor(x => x.ControlId)
                        .NotEmpty()
                        .WithMessage("Control reference is required")
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
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("ASSET.MERCHANT_REQUIRED");

        }
    }
}
