using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.EvidenceType
{
    public class UpdateEvidenceTypeRequest : BaseRequest
    {
        public EvidenceTypeDto EvidenceType { get; set; }
    }

    public class UpdateEvidenceTypeValidator : AbstractValidator<EvidenceTypeDto>
    {
        public UpdateEvidenceTypeValidator()
        {
            RuleFor(x => x.EvidenceTypeId)
                .NotEmpty()
                .WithMessage("Evidence Type Id is required")
                .WithErrorCode("EVIDENCETYPE.EVIDENCETYPEID_REQUIRED");

            RuleFor(x => x.EvidenceTypeCode)
                .NotEmpty()
                .WithMessage("Evidence Type Code is required")
                .WithErrorCode("EVIDENCETYPE.EVIDENCETYPECODE_REQUIRED")
                .MaximumLength(30)
                .WithMessage("EvidenceTypeCode cannot exceed 30 characters")
                .WithErrorCode("EVIDENCETYPE.EVIDENCETYPECODE_LENGTH")
                ;

            RuleFor(x => x.EvidenceTypeName)
                .NotEmpty()
                .WithMessage("Evidence Type Name is required")
                .WithErrorCode("EVIDENCETYPE.EVIDENCETYPENAME_REQUIRED")
                .MaximumLength(100)
                .WithMessage("EvidenceTypeName cannot exceed 100 characters")
                .WithErrorCode("EVIDENCETYPE.EVIDENCETYPENAME_LENGTH")
                ;

            RuleFor(x => x.FileExtensions)
                .MaximumLength(200)
                .WithMessage("FileExtensions cannot exceed 200 characters")
                .WithErrorCode("EVIDENCETYPE.FILEEXTENSIONS_LENGTH")
                .Must(ext => new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" }.Contains(ext?.ToLower()))
                .WithMessage("FileExtensions must be a valid document extension")
                .WithErrorCode("EVIDENCETYPE.FILEEXTENSIONS_EXTENSION_INVALID")
                ;

            RuleFor(x => x.MaxSizeMB)
                .InclusiveBetween(0, 1000)
                .WithMessage("MaxSizeMB must be between 0 and 1000")
                .WithErrorCode("EVIDENCETYPE.MAXSIZEMB_RANGE_INVALID")
                ;

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Is Active is required")
                .WithErrorCode("EVIDENCETYPE.ISACTIVE_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsActive transition is not allowed")
                .WithErrorCode("EVIDENCETYPE.ISACTIVE_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsActive due to business rules")
                .WithErrorCode("EVIDENCETYPE.ISACTIVE_STATUS_RULES_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("EVIDENCETYPE.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("EVIDENCETYPE.CREATEDBY_REQUIRED")
                ;

        }
    }
}
