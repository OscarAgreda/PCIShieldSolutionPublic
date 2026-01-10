using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.NetworkSegmentation
{
    public class UpdateNetworkSegmentationRequest : BaseRequest
    {
        public NetworkSegmentationDto NetworkSegmentation { get; set; }
    }

    public class UpdateNetworkSegmentationValidator : AbstractValidator<NetworkSegmentationDto>
    {
        public UpdateNetworkSegmentationValidator()
        {
            RuleFor(x => x.NetworkSegmentationId)
                .NotEmpty()
                .WithMessage("Network Segmentation Id is required")
                .WithErrorCode("NETWORKSEGMENTATION.NETWORKSEGMENTATIONID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("NETWORKSEGMENTATION.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("NETWORKSEGMENTATION.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.SegmentName)
                .NotEmpty()
                .WithMessage("Segment Name is required")
                .WithErrorCode("NETWORKSEGMENTATION.SEGMENTNAME_REQUIRED")
                .MaximumLength(100)
                .WithMessage("SegmentName cannot exceed 100 characters")
                .WithErrorCode("NETWORKSEGMENTATION.SEGMENTNAME_LENGTH")
                ;

            RuleFor(x => x.IPRange)
                .NotEmpty()
                .WithMessage("Iprange is required")
                .WithErrorCode("NETWORKSEGMENTATION.IPRANGE_REQUIRED")
                .MaximumLength(50)
                .WithMessage("IPRange cannot exceed 50 characters")
                .WithErrorCode("NETWORKSEGMENTATION.IPRANGE_LENGTH")
                .Matches(@"^(?:(?:25[0-5]|2[0-4]\d|1?\d{1,2})\.){3}(?:25[0-5]|2[0-4]\d|1?\d{1,2})(?:/(?:(?:[0-9]|[1-2][0-9]|3[0-2])|(?:(?:25[0-5]|2[0-4]\d|1?\d{1,2})\.){3}(?:25[0-5]|2[0-4]\d|1?\d{1,2})))?$")
                .WithMessage("IPRange has an invalid format")
                .WithErrorCode("NETWORKSEGMENTATION.IPRANGE_FORMAT")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("IPRange has an invalid format")
                .WithErrorCode("NETWORKSEGMENTATION.IPRANGE_FORMAT_INVALID")
                ;

            RuleFor(x => x.IsInCDE)
                .NotNull()
                .WithMessage("Is In CDE is required")
                .WithErrorCode("NETWORKSEGMENTATION.ISINCDE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsInCDE transition is not allowed")
                .WithErrorCode("NETWORKSEGMENTATION.ISINCDE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsInCDE due to business rules")
                .WithErrorCode("NETWORKSEGMENTATION.ISINCDE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("NETWORKSEGMENTATION.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("NETWORKSEGMENTATION.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("NETWORKSEGMENTATION.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("NETWORKSEGMENTATION.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("NETWORKSEGMENTATION.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("NETWORKSEGMENTATION.MERCHANT_REQUIRED");

        }
    }
}
