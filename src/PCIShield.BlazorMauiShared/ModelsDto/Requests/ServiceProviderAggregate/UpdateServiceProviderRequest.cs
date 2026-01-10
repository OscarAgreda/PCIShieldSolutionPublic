using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.ServiceProvider
{
    public class UpdateServiceProviderRequest : BaseRequest
    {
        public ServiceProviderDto ServiceProvider { get; set; }
    }

    public class UpdateServiceProviderValidator : AbstractValidator<ServiceProviderDto>
    {
        public UpdateServiceProviderValidator()
        {
            RuleFor(x => x.ServiceProviderId)
                .NotEmpty()
                .WithMessage("Service Provider Id is required")
                .WithErrorCode("SERVICEPROVIDER.SERVICEPROVIDERID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("SERVICEPROVIDER.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("SERVICEPROVIDER.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.ProviderName)
                .NotEmpty()
                .WithMessage("Provider Name is required")
                .WithErrorCode("SERVICEPROVIDER.PROVIDERNAME_REQUIRED")
                .MaximumLength(200)
                .WithMessage("ProviderName cannot exceed 200 characters")
                .WithErrorCode("SERVICEPROVIDER.PROVIDERNAME_LENGTH")
                ;

            RuleFor(x => x.ServiceType)
                .NotEmpty()
                .WithMessage("Service Type is required")
                .WithErrorCode("SERVICEPROVIDER.SERVICETYPE_REQUIRED")
                .MaximumLength(100)
                .WithMessage("ServiceType cannot exceed 100 characters")
                .WithErrorCode("SERVICEPROVIDER.SERVICETYPE_LENGTH")
                ;

            RuleFor(x => x.IsPCICompliant)
                .NotNull()
                .WithMessage("Is Pcicompliant is required")
                .WithErrorCode("SERVICEPROVIDER.ISPCICOMPLIANT_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsPCICompliant transition is not allowed")
                .WithErrorCode("SERVICEPROVIDER.ISPCICOMPLIANT_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsPCICompliant due to business rules")
                .WithErrorCode("SERVICEPROVIDER.ISPCICOMPLIANT_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("SERVICEPROVIDER.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("SERVICEPROVIDER.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("SERVICEPROVIDER.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("SERVICEPROVIDER.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("SERVICEPROVIDER.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("SERVICEPROVIDER.MERCHANT_REQUIRED");

        }
    }
}
