using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.PaymentPage;

namespace PCIShield.BlazorMauiShared.Models.PaymentChannel
{
    public class UpdatePaymentChannelRequest : BaseRequest
    {
        public PaymentChannelDto PaymentChannel { get; set; }
    }

    public class UpdatePaymentChannelValidator : AbstractValidator<PaymentChannelDto>
    {
        public UpdatePaymentChannelValidator()
        {
            RuleFor(x => x.PaymentChannelId)
                .NotEmpty()
                .WithMessage("Payment Channel Id is required")
                .WithErrorCode("PAYMENTCHANNEL.PAYMENTCHANNELID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("PAYMENTCHANNEL.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("PAYMENTCHANNEL.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.ChannelCode)
                .NotEmpty()
                .WithMessage("Channel Code is required")
                .WithErrorCode("PAYMENTCHANNEL.CHANNELCODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("ChannelCode cannot exceed 30 characters")
                .WithErrorCode("PAYMENTCHANNEL.CHANNELCODE_LENGTH")
                ;

            RuleFor(x => x.ChannelName)
                .NotEmpty()
                .WithMessage("Channel Name is required")
                .WithErrorCode("PAYMENTCHANNEL.CHANNELNAME_REQUIRED")
                .MaximumLength(100)
                .WithMessage("ChannelName cannot exceed 100 characters")
                .WithErrorCode("PAYMENTCHANNEL.CHANNELNAME_LENGTH")
                ;

            RuleFor(x => x.ChannelType)
                .NotEmpty()
                .WithMessage("Channel Type is required")
                .WithErrorCode("PAYMENTCHANNEL.CHANNELTYPE_REQUIRED")
                ;

            RuleFor(x => x.ProcessingVolume)
                .NotEmpty()
                .WithMessage("Processing Volume is required")
                .WithErrorCode("PAYMENTCHANNEL.PROCESSINGVOLUME_REQUIRED")
                .InclusiveBetween(0M, 1000M)
                .WithMessage("ProcessingVolume must be between 0 and 1000")
                .WithErrorCode("PAYMENTCHANNEL.PROCESSINGVOLUME_RANGE_INVALID")
                ;

            RuleFor(x => x.IsInScope)
                .NotNull()
                .WithMessage("Is In Scope is required")
                .WithErrorCode("PAYMENTCHANNEL.ISINSCOPE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsInScope transition is not allowed")
                .WithErrorCode("PAYMENTCHANNEL.ISINSCOPE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsInScope due to business rules")
                .WithErrorCode("PAYMENTCHANNEL.ISINSCOPE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.TokenizationEnabled)
                .NotNull()
                .WithMessage("Tokenization Enabled is required")
                .WithErrorCode("PAYMENTCHANNEL.TOKENIZATIONENABLED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("TokenizationEnabled transition is not allowed")
                .WithErrorCode("PAYMENTCHANNEL.TOKENIZATIONENABLED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change TokenizationEnabled due to business rules")
                .WithErrorCode("PAYMENTCHANNEL.TOKENIZATIONENABLED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("PAYMENTCHANNEL.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("PAYMENTCHANNEL.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("PAYMENTCHANNEL.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("PAYMENTCHANNEL.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("PAYMENTCHANNEL.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("PAYMENTCHANNEL.MERCHANT_REQUIRED");

        }
    }
}
