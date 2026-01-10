using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.BlazorMauiShared.Models.Asset;
using PCIShield.BlazorMauiShared.Models.CompensatingControl;
using PCIShield.BlazorMauiShared.Models.ComplianceOfficer;
using PCIShield.BlazorMauiShared.Models.CryptographicInventory;
using PCIShield.BlazorMauiShared.Models.Evidence;
using PCIShield.BlazorMauiShared.Models.NetworkSegmentation;
using PCIShield.BlazorMauiShared.Models.PaymentChannel;
using PCIShield.BlazorMauiShared.Models.ServiceProvider;

namespace PCIShield.BlazorMauiShared.Models.Merchant
{
    public class UpdateMerchantRequest : BaseRequest
    {
        public MerchantDto Merchant { get; set; }
    }

    public class UpdateMerchantValidator : AbstractValidator<MerchantDto>
    {
        public UpdateMerchantValidator()
        {
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("MERCHANT.MERCHANTID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("MERCHANT.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantCode)
                .NotEmpty()
                .WithMessage("Merchant Code is required")
                .WithErrorCode("MERCHANT.MERCHANTCODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("MerchantCode cannot exceed 30 characters")
                .WithErrorCode("MERCHANT.MERCHANTCODE_LENGTH")
                ;

            RuleFor(x => x.MerchantName)
                .NotEmpty()
                .WithMessage("Merchant Name is required")
                .WithErrorCode("MERCHANT.MERCHANTNAME_REQUIRED")
                .MaximumLength(200)
                .WithMessage("MerchantName cannot exceed 200 characters")
                .WithErrorCode("MERCHANT.MERCHANTNAME_LENGTH")
                ;

            RuleFor(x => x.MerchantLevel)
                .NotEmpty()
                .WithMessage("Merchant Level is required")
                .WithErrorCode("MERCHANT.MERCHANTLEVEL_REQUIRED")
                .InclusiveBetween(0, 100)
                .WithMessage("MerchantLevel must be between 0 and 100")
                .WithErrorCode("MERCHANT.MERCHANTLEVEL_RANGE_INVALID")
                ;

            RuleFor(x => x.AcquirerName)
                .NotEmpty()
                .WithMessage("Acquirer Name is required")
                .WithErrorCode("MERCHANT.ACQUIRERNAME_REQUIRED")
                .MaximumLength(200)
                .WithMessage("AcquirerName cannot exceed 200 characters")
                .WithErrorCode("MERCHANT.ACQUIRERNAME_LENGTH")
                ;

            RuleFor(x => x.ProcessorMID)
                .NotEmpty()
                .WithMessage("Processor MID is required")
                .WithErrorCode("MERCHANT.PROCESSORMID_REQUIRED")
                .MaximumLength(50)
                .WithMessage("ProcessorMID cannot exceed 50 characters")
                .WithErrorCode("MERCHANT.PROCESSORMID_LENGTH")
                ;

            RuleFor(x => x.AnnualCardVolume)
                .NotEmpty()
                .WithMessage("Annual Card Volume is required")
                .WithErrorCode("MERCHANT.ANNUALCARDVOLUME_REQUIRED")
                .InclusiveBetween(0M, 1000M)
                .WithMessage("AnnualCardVolume must be between 0 and 1000")
                .WithErrorCode("MERCHANT.ANNUALCARDVOLUME_RANGE_INVALID")
                ;

            RuleFor(x => x.NextAssessmentDue)
                .NotNull()
                .WithMessage("Next Assessment Due is required")
                .WithErrorCode("MERCHANT.NEXTASSESSMENTDUE_REQUIRED")
                ;

            RuleFor(x => x.ComplianceRank)
                .NotEmpty()
                .WithMessage("Compliance Rank is required")
                .WithErrorCode("MERCHANT.COMPLIANCERANK_REQUIRED")
                .InclusiveBetween(0, 100)
                .WithMessage("ComplianceRank must be between 0 and 100")
                .WithErrorCode("MERCHANT.COMPLIANCERANK_RANGE_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("MERCHANT.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("MERCHANT.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("MERCHANT.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("MERCHANT.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("MERCHANT.ISDELETED_STATUS_RULES_INVALID")
                ;

        }
    }
}
