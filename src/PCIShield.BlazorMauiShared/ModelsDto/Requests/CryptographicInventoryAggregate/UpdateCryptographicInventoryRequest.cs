using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.CryptographicInventory
{
    public class UpdateCryptographicInventoryRequest : BaseRequest
    {
        public CryptographicInventoryDto CryptographicInventory { get; set; }
    }

    public class UpdateCryptographicInventoryValidator : AbstractValidator<CryptographicInventoryDto>
    {
        public UpdateCryptographicInventoryValidator()
        {
            RuleFor(x => x.CryptographicInventoryId)
                .NotEmpty()
                .WithMessage("Cryptographic Inventory Id is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.CRYPTOGRAPHICINVENTORYID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.MerchantId)
                .NotNull()
                .WithMessage("Merchant Id is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.MERCHANTID_REQUIRED")
                ;

            RuleFor(x => x.KeyName)
                .NotEmpty()
                .WithMessage("Key Name is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYNAME_REQUIRED")
                .MaximumLength(200)
                .WithMessage("KeyName cannot exceed 200 characters")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYNAME_LENGTH")
                ;

            RuleFor(x => x.KeyType)
                .NotEmpty()
                .WithMessage("Key Type is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYTYPE_REQUIRED")
                .MaximumLength(50)
                .WithMessage("KeyType cannot exceed 50 characters")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYTYPE_LENGTH")
                ;

            RuleFor(x => x.Algorithm)
                .NotEmpty()
                .WithMessage("Algorithm is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.ALGORITHM_REQUIRED")
                .MaximumLength(50)
                .WithMessage("Algorithm cannot exceed 50 characters")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.ALGORITHM_LENGTH")
                ;

            RuleFor(x => x.KeyLength)
                .NotEmpty()
                .WithMessage("Key Length is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYLENGTH_REQUIRED")
                .InclusiveBetween(0, 1000)
                .WithMessage("KeyLength must be between 0 and 1000")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYLENGTH_RANGE_INVALID")
                ;

            RuleFor(x => x.KeyLocation)
                .NotEmpty()
                .WithMessage("Key Location is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYLOCATION_REQUIRED")
                .MaximumLength(200)
                .WithMessage("KeyLocation cannot exceed 200 characters")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.KEYLOCATION_LENGTH")
                ;

            RuleFor(x => x.CreationDate)
                .NotNull()
                .WithMessage("Creation Date is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.CREATIONDATE_REQUIRED")
                ;

            RuleFor(x => x.NextRotationDue)
                .NotNull()
                .WithMessage("Next Rotation Due is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.NEXTROTATIONDUE_REQUIRED")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.MerchantId)
                .NotEmpty()
                .WithMessage("Merchant reference is required")
                .WithErrorCode("CRYPTOGRAPHICINVENTORY.MERCHANT_REQUIRED");

        }
    }
}
