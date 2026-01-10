using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.ApplicationUser
{
    public class UpdateApplicationUserRequest : BaseRequest
    {
        public ApplicationUserDto ApplicationUser { get; set; }
    }

    public class UpdateApplicationUserValidator : AbstractValidator<ApplicationUserDto>
    {
        public UpdateApplicationUserValidator()
        {
            RuleFor(x => x.ApplicationUserId)
                .NotEmpty()
                .WithMessage("Application User Id is required")
                .WithErrorCode("APPLICATIONUSER.APPLICATIONUSERID_REQUIRED");

            RuleFor(x => x.FirstName)
                .MaximumLength(255)
                .WithMessage("FirstName cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.FIRSTNAME_LENGTH")
                ;

            RuleFor(x => x.LastName)
                .MaximumLength(255)
                .WithMessage("LastName cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.LASTNAME_LENGTH")
                ;

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("User Name is required")
                .WithErrorCode("APPLICATIONUSER.USERNAME_REQUIRED")
                .MaximumLength(255)
                .WithMessage("UserName cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.USERNAME_LENGTH")
                ;

            RuleFor(x => x.CreatedDate)
                .NotNull()
                .WithMessage("Created Date is required")
                .WithErrorCode("APPLICATIONUSER.CREATEDDATE_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("APPLICATIONUSER.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsLoginAllowed)
                .NotNull()
                .WithMessage("Is Login Allowed is required")
                .WithErrorCode("APPLICATIONUSER.ISLOGINALLOWED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsLoginAllowed transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISLOGINALLOWED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsLoginAllowed due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISLOGINALLOWED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.FailedLoginCount)
                .NotEmpty()
                .WithMessage("Failed Login Count is required")
                .WithErrorCode("APPLICATIONUSER.FAILEDLOGINCOUNT_REQUIRED")
                .InclusiveBetween(0, 1000)
                .WithMessage("FailedLoginCount must be between 0 and 1000")
                .WithErrorCode("APPLICATIONUSER.FAILEDLOGINCOUNT_RANGE_INVALID")
                ;

            RuleFor(x => x.Email)
                .MaximumLength(255)
                .WithMessage("Email cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.EMAIL_LENGTH")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Email has an invalid format")
                .WithErrorCode("APPLICATIONUSER.EMAIL_FORMAT")
                .Must(email =>
            {
                try
                {
                    var addr = new MailAddress(email);
                    if (!string.Equals(addr.Address, email, StringComparison.Ordinal)) return false;
                    var parts = email.Split('@');
                    if (parts.Length != 2) return false;

                    var local = parts[0];
                    var domain = parts[1];
                    if (local.Length == 0 || local.Length > 64) return false;
                    if (email.Length > 254) return false;
                    if (!domain.Contains('.')) return false;

                    return true;
                }
                catch
                {
                    return false;
                }
            })
                .WithMessage("Email must be a valid email address")
                .WithErrorCode("APPLICATIONUSER.EMAIL_EMAIL_INVALID")
                ;

            RuleFor(x => x.Phone)
                .MaximumLength(255)
                .WithMessage("Phone cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.PHONE_LENGTH")
                .Must(phone => {
                if (string.IsNullOrEmpty(phone)) return true;
                return System.Text.RegularExpressions.Regex.IsMatch(
                    phone,
                    @"^\+?[1-9]\d{1,14}$"
                );
            })
                .WithMessage("Phone must be a valid international phone number")
                .WithErrorCode("APPLICATIONUSER.PHONE_PHONE_INVALID")
                ;

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(255)
                .WithMessage("AvatarUrl cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.AVATARURL_LENGTH")
                .Must(url => {
                if (string.IsNullOrEmpty(url)) return true;
                return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                       (uriResult.Scheme == Uri.UriSchemeHttp || 
                        uriResult.Scheme == Uri.UriSchemeHttps) &&
                       !url.Contains(" ");
            })
                .WithMessage("AvatarUrl must be a valid HTTP/HTTPS URL")
                .WithErrorCode("APPLICATIONUSER.AVATARURL_URL_INVALID")
                ;

            RuleFor(x => x.IsUserApproved)
                .NotNull()
                .WithMessage("Is User Approved is required")
                .WithErrorCode("APPLICATIONUSER.ISUSERAPPROVED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsUserApproved transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISUSERAPPROVED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsUserApproved due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISUSERAPPROVED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsPhoneVerified)
                .NotNull()
                .WithMessage("Is Phone Verified is required")
                .WithErrorCode("APPLICATIONUSER.ISPHONEVERIFIED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsPhoneVerified transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISPHONEVERIFIED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsPhoneVerified due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISPHONEVERIFIED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsEmailVerified)
                .NotNull()
                .WithMessage("Is Email Verified is required")
                .WithErrorCode("APPLICATIONUSER.ISEMAILVERIFIED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsEmailVerified transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISEMAILVERIFIED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsEmailVerified due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISEMAILVERIFIED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.ConfirmationEmail)
                .MaximumLength(255)
                .WithMessage("ConfirmationEmail cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.CONFIRMATIONEMAIL_LENGTH")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("ConfirmationEmail has an invalid format")
                .WithErrorCode("APPLICATIONUSER.CONFIRMATIONEMAIL_FORMAT")
                .Must(email =>
            {
                try
                {
                    var addr = new MailAddress(email);
                    if (!string.Equals(addr.Address, email, StringComparison.Ordinal)) return false;
                    var parts = email.Split('@');
                    if (parts.Length != 2) return false;

                    var local = parts[0];
                    var domain = parts[1];
                    if (local.Length == 0 || local.Length > 64) return false;
                    if (email.Length > 254) return false;
                    if (!domain.Contains('.')) return false;

                    return true;
                }
                catch
                {
                    return false;
                }
            })
                .WithMessage("ConfirmationEmail must be a valid email address")
                .WithErrorCode("APPLICATIONUSER.CONFIRMATIONEMAIL_EMAIL_INVALID")
                ;

            RuleFor(x => x.IsLocked)
                .NotNull()
                .WithMessage("Is Locked is required")
                .WithErrorCode("APPLICATIONUSER.ISLOCKED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsLocked transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISLOCKED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsLocked due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISLOCKED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("APPLICATIONUSER.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISDELETED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsUserFullyRegistered)
                .NotNull()
                .WithMessage("Is User Fully Registered is required")
                .WithErrorCode("APPLICATIONUSER.ISUSERFULLYREGISTERED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsUserFullyRegistered transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISUSERFULLYREGISTERED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsUserFullyRegistered due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISUSERFULLYREGISTERED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.AvailabilityRank)
                .MaximumLength(50)
                .WithMessage("AvailabilityRank cannot exceed 50 characters")
                .WithErrorCode("APPLICATIONUSER.AVAILABILITYRANK_LENGTH")
                ;

            RuleFor(x => x.IsBanned)
                .NotNull()
                .WithMessage("Is Banned is required")
                .WithErrorCode("APPLICATIONUSER.ISBANNED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsBanned transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISBANNED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsBanned due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISBANNED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsFullyRegistered)
                .NotNull()
                .WithMessage("Is Fully Registered is required")
                .WithErrorCode("APPLICATIONUSER.ISFULLYREGISTERED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsFullyRegistered transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISFULLYREGISTERED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsFullyRegistered due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISFULLYREGISTERED_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.LastLoginIP)
                .MaximumLength(50)
                .WithMessage("LastLoginIP cannot exceed 50 characters")
                .WithErrorCode("APPLICATIONUSER.LASTLOGINIP_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("LastLoginIP has an invalid format")
                .WithErrorCode("APPLICATIONUSER.LASTLOGINIP_FORMAT")
                ;

            RuleFor(x => x.IsOnline)
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsOnline transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISONLINE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (currentValue.HasValue && (bool)currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsOnline due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISONLINE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsConnectedToSignalr)
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsConnectedToSignalr transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISCONNECTEDTOSIGNALR_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (currentValue.HasValue && (bool)currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsConnectedToSignalr due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISCONNECTEDTOSIGNALR_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsLoggedIntoApp)
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsLoggedIntoApp transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISLOGGEDINTOAPP_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (currentValue.HasValue && (bool)currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsLoggedIntoApp due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISLOGGEDINTOAPP_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.UserIconUrl)
                .MaximumLength(255)
                .WithMessage("UserIconUrl cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.USERICONURL_LENGTH")
                .Must(url => {
                if (string.IsNullOrEmpty(url)) return true;
                return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                       (uriResult.Scheme == Uri.UriSchemeHttp || 
                        uriResult.Scheme == Uri.UriSchemeHttps) &&
                       !url.Contains(" ");
            })
                .WithMessage("UserIconUrl must be a valid HTTP/HTTPS URL")
                .WithErrorCode("APPLICATIONUSER.USERICONURL_URL_INVALID")
                ;

            RuleFor(x => x.UserProfileImagePath)
                .MaximumLength(255)
                .WithMessage("UserProfileImagePath cannot exceed 255 characters")
                .WithErrorCode("APPLICATIONUSER.USERPROFILEIMAGEPATH_LENGTH")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("APPLICATIONUSER.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("APPLICATIONUSER.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.IsEmployee)
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsEmployee transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISEMPLOYEE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (currentValue.HasValue && (bool)currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsEmployee due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISEMPLOYEE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsErpOwner)
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsErpOwner transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISERPOWNER_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (currentValue.HasValue && (bool)currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsErpOwner due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISERPOWNER_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.IsCustomer)
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsCustomer transition is not allowed")
                .WithErrorCode("APPLICATIONUSER.ISCUSTOMER_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (currentValue.HasValue && (bool)currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsCustomer due to business rules")
                .WithErrorCode("APPLICATIONUSER.ISCUSTOMER_STATUS_RULES_INVALID")
                ;

        }
    }
}
