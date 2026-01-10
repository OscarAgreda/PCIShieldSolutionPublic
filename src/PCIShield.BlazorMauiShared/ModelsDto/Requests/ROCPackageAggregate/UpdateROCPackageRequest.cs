using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.ROCPackage
{
    public class UpdateROCPackageRequest : BaseRequest
    {
        public ROCPackageDto ROCPackage { get; set; }
    }

    public class UpdateROCPackageValidator : AbstractValidator<ROCPackageDto>
    {
        public UpdateROCPackageValidator()
        {
            RuleFor(x => x.ROCPackageId)
                .NotEmpty()
                .WithMessage("Rocpackage Id is required")
                .WithErrorCode("ROCPACKAGE.ROCPACKAGEID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("ROCPACKAGE.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.AssessmentId)
                .NotNull()
                .WithMessage("Assessment Id is required")
                .WithErrorCode("ROCPACKAGE.ASSESSMENTID_REQUIRED")
                ;

            RuleFor(x => x.PackageVersion)
                .NotEmpty()
                .WithMessage("Package Version is required")
                .WithErrorCode("ROCPACKAGE.PACKAGEVERSION_REQUIRED")
                .MaximumLength(20)
                .WithMessage("PackageVersion cannot exceed 20 characters")
                .WithErrorCode("ROCPACKAGE.PACKAGEVERSION_LENGTH")
                ;

            RuleFor(x => x.GeneratedDate)
                .NotNull()
                .WithMessage("Generated Date is required")
                .WithErrorCode("ROCPACKAGE.GENERATEDDATE_REQUIRED")
                ;

            RuleFor(x => x.QSAName)
                .MaximumLength(200)
                .WithMessage("QSAName cannot exceed 200 characters")
                .WithErrorCode("ROCPACKAGE.QSANAME_LENGTH")
                ;

            RuleFor(x => x.QSACompany)
                .MaximumLength(200)
                .WithMessage("QSACompany cannot exceed 200 characters")
                .WithErrorCode("ROCPACKAGE.QSACOMPANY_LENGTH")
                ;

            RuleFor(x => x.AOCNumber)
                .MaximumLength(50)
                .WithMessage("AOCNumber cannot exceed 50 characters")
                .WithErrorCode("ROCPACKAGE.AOCNUMBER_LENGTH")
                ;

            RuleFor(x => x.Rank)
                .NotEmpty()
                .WithMessage("Rank is required")
                .WithErrorCode("ROCPACKAGE.RANK_REQUIRED")
                .InclusiveBetween(0, 100)
                .WithMessage("Rank must be between 0 and 100")
                .WithErrorCode("ROCPACKAGE.RANK_RANGE_INVALID")
                ;

            RuleFor(x => x.CreatedAt)
                .NotNull()
                .WithMessage("Created At is required")
                .WithErrorCode("ROCPACKAGE.CREATEDAT_REQUIRED")
                ;

            RuleFor(x => x.CreatedBy)
                .NotNull()
                .WithMessage("Created By is required")
                .WithErrorCode("ROCPACKAGE.CREATEDBY_REQUIRED")
                ;

            RuleFor(x => x.IsDeleted)
                .NotNull()
                .WithMessage("Is Deleted is required")
                .WithErrorCode("ROCPACKAGE.ISDELETED_REQUIRED")
                .Must((model, currentValue) => StatusValidationHelper.ValidateStatusChange(model, currentValue))
                .WithMessage("IsDeleted transition is not allowed")
                .WithErrorCode("ROCPACKAGE.ISDELETED_STATUS_TRANSITION_INVALID")
                .Must((model, currentValue) => {
                   if (!currentValue)
                       return !StatusValidationHelper.HasActiveReferences(model);
                   return StatusValidationHelper.HasRequiredActiveData(model);
               })
                .WithMessage("Cannot change IsDeleted due to business rules")
                .WithErrorCode("ROCPACKAGE.ISDELETED_STATUS_RULES_INVALID")
                ;
            RuleFor(x => x.AssessmentId)
                .NotEmpty()
                .WithMessage("Assessment reference is required")
                .WithErrorCode("ROCPACKAGE.ASSESSMENT_REQUIRED");

        }
    }
}
