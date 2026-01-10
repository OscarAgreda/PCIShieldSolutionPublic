using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.Script;

namespace PCIShield.BlazorMauiShared.Models.PaymentPage
{
    public class UpdatePaymentPageRequest : BaseRequest
    {
        public PaymentPageDto PaymentPage { get; set; }
    }

    public class UpdatePaymentPageValidator : AbstractValidator<PaymentPageDto>
    {
        public UpdatePaymentPageValidator()
        {
            RuleFor(x => x.PaymentPageId)
                .NotEmpty()
                .WithMessage("Payment Page Id is required")
                .WithErrorCode("PAYMENTPAGE.PAYMENTPAGEID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("PAYMENTPAGE.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.PaymentChannelId)
                .NotNull()
                .WithMessage("Payment Channel Id is required")
                .WithErrorCode("PAYMENTPAGE.PAYMENTCHANNELID_REQUIRED")
                ;

            RuleFor(x => x.PageUrl)
                .NotEmpty()
                .WithMessage("Page Url is required")
                .WithErrorCode("PAYMENTPAGE.PAGEURL_REQUIRED")
                .MaximumLength(500)
                .WithMessage("PageUrl cannot exceed 500 characters")
                .WithErrorCode("PAYMENTPAGE.PAGEURL_LENGTH")
                .Must(url => {
                if (string.IsNullOrEmpty(url)) return true;
                return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                       (uriResult.Scheme == Uri.UriSchemeHttp || 
                        uriResult.Scheme == Uri.UriSchemeHttps) &&
                       !url.Contains(" ");
            })
                .WithMessage("PageUrl must be a valid HTTP/HTTPS URL")
                .WithErrorCode("PAYMENTPAGE.PAGEURL_URL_INVALID")
                ;

            RuleFor(x => x.PageName)
                .NotEmpty()
                .WithMessage("Page Name is required")
                .WithErrorCode("PAYMENTPAGE.PAGENAME_REQUIRED")
                .MaximumLength(200)
                .WithMessage("PageName cannot exceed 200 characters")
                .WithErrorCode("PAYMENTPAGE.PAGENAME_LENGTH")
                ;

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Is Active is required")
                .WithErrorCode("PAYMENTPAGE.ISACTIVE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsActive transition is not allowed")
                .WithErrorCode("PAYMENTPAGE.ISACTIVE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsActive due to business rules")
                .WithErrorCode("PAYMENTPAGE.ISACTIVE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.ScriptIntegrityHash)
                .MaximumLength(128)
                .WithMessage("ScriptIntegrityHash cannot exceed 128 characters")
                .WithErrorCode("PAYMENTPAGE.SCRIPTINTEGRITYHASH_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("ScriptIntegrityHash has an invalid format")
                .WithErrorCode("PAYMENTPAGE.SCRIPTINTEGRITYHASH_FORMAT")
                .Must(password => {
                        if (string.IsNullOrEmpty(password)) return true;
                        var hasNumber = new System.Text.RegularExpressions.Regex(@"[0-9]+");
                        var hasUpperChar = new System.Text.RegularExpressions.Regex(@"[A-Z]+");
                        var hasLowerChar = new System.Text.RegularExpressions.Regex(@"[a-z]+");
                        var hasSymbols = new System.Text.RegularExpressions.Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
                        
                        return password.Length >= 8 &&
                               hasNumber.IsMatch(password) &&
                               hasUpperChar.IsMatch(password) &&
                               hasLowerChar.IsMatch(password) &&
                               hasSymbols.IsMatch(password);
                    })
                .WithMessage("Password must be at least 8 characters and contain uppercase, lowercase, number, and special characters")
                .WithErrorCode("PAYMENTPAGE.SCRIPTINTEGRITYHASH_PASSWORD_STRENGTH_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("PAYMENTPAGE.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("PAYMENTPAGE.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("PAYMENTPAGE.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("PAYMENTPAGE.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("PAYMENTPAGE.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.PaymentChannelId)
                .NotEmpty()
                .WithMessage("PaymentChannel reference is required")
                .WithErrorCode("PAYMENTPAGE.PAYMENTCHANNEL_REQUIRED");
            RuleFor(x => x.PaymentChannel)
                .Must(parent => Vali.ValidateParentStatus(parent))
                .When(x => x.IsActive)
                .WithMessage("PaymentChannel must be in valid status")
                .WithErrorCode("PAYMENTPAGE.PAYMENTCHANNEL_STATUS");
        }
    }
}
