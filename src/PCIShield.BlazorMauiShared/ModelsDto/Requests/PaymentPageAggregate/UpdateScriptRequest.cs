using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.Script
{
    public class UpdateScriptRequest : BaseRequest
    {
        public ScriptDto Script { get; set; }
    }

    public class UpdateScriptValidator : AbstractValidator<ScriptDto>
    {
        public UpdateScriptValidator()
        {
            RuleFor(x => x.ScriptId)
                .NotEmpty()
                .WithMessage("Script Id is required")
                .WithErrorCode("SCRIPT.SCRIPTID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("SCRIPT.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.PaymentPageId)
                .NotNull()
                .WithMessage("Payment Page Id is required")
                .WithErrorCode("SCRIPT.PAYMENTPAGEID_REQUIRED")
                ;

            RuleFor(x => x.ScriptUrl)
                .NotEmpty()
                .WithMessage("Script Url is required")
                .WithErrorCode("SCRIPT.SCRIPTURL_REQUIRED")
                .MaximumLength(500)
                .WithMessage("ScriptUrl cannot exceed 500 characters")
                .WithErrorCode("SCRIPT.SCRIPTURL_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("ScriptUrl has an invalid format")
                .WithErrorCode("SCRIPT.SCRIPTURL_FORMAT")
                .Must(url => {
                if (string.IsNullOrEmpty(url)) return true;
                return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                       (uriResult.Scheme == Uri.UriSchemeHttp || 
                        uriResult.Scheme == Uri.UriSchemeHttps) &&
                       !url.Contains(" ");
            })
                .WithMessage("ScriptUrl must be a valid HTTP/HTTPS URL")
                .WithErrorCode("SCRIPT.SCRIPTURL_URL_INVALID")
                ;

            RuleFor(x => x.ScriptHash)
                .NotEmpty()
                .WithMessage("Script Hash is required")
                .WithErrorCode("SCRIPT.SCRIPTHASH_REQUIRED")
                .MaximumLength(128)
                .WithMessage("ScriptHash cannot exceed 128 characters")
                .WithErrorCode("SCRIPT.SCRIPTHASH_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("ScriptHash has an invalid format")
                .WithErrorCode("SCRIPT.SCRIPTHASH_FORMAT")
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
                .WithErrorCode("SCRIPT.SCRIPTHASH_PASSWORD_STRENGTH_INVALID")
                ;

            RuleFor(x => x.ScriptType)
                .NotEmpty()
                .WithMessage("Script Type is required")
                .WithErrorCode("SCRIPT.SCRIPTTYPE_REQUIRED")
                .MaximumLength(50)
                .WithMessage("ScriptType cannot exceed 50 characters")
                .WithErrorCode("SCRIPT.SCRIPTTYPE_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("ScriptType has an invalid format")
                .WithErrorCode("SCRIPT.SCRIPTTYPE_FORMAT")
                ;

            RuleFor(x => x.IsAuthorized)
                .NotNull()
                .WithMessage("Is Authorized is required")
                .WithErrorCode("SCRIPT.ISAUTHORIZED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsAuthorized transition is not allowed")
                .WithErrorCode("SCRIPT.ISAUTHORIZED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsAuthorized due to business rules")
                .WithErrorCode("SCRIPT.ISAUTHORIZED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.FirstSeen)
                .NotNull()
                .WithMessage("First Seen is required")
                .WithErrorCode("SCRIPT.FIRSTSEEN_REQUIRED")
                ;

            RuleFor(x => x.LastSeen)
                .NotNull()
                .WithMessage("Last Seen is required")
                .WithErrorCode("SCRIPT.LASTSEEN_REQUIRED")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("SCRIPT.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("SCRIPT.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("SCRIPT.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("SCRIPT.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("SCRIPT.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.PaymentPageId)
                .NotEmpty()
                .WithMessage("PaymentPage reference is required")
                .WithErrorCode("SCRIPT.PAYMENTPAGE_REQUIRED");
            RuleFor(x => x.PaymentPage)
                .Must(parent => parent != null && parent.IsActive)
                .When(x => x.PaymentPageId != null)
                .WithMessage("PaymentPage must be active")
                .WithErrorCode("SCRIPT.PAYMENTPAGE_INACTIVE");

        }
    }
}
