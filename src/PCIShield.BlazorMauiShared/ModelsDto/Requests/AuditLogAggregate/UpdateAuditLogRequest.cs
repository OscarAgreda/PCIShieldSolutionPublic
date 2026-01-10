using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.AuditLog
{
    public class UpdateAuditLogRequest : BaseRequest
    {
        public AuditLogDto AuditLog { get; set; }
    }

    public class UpdateAuditLogValidator : AbstractValidator<AuditLogDto>
    {
        public UpdateAuditLogValidator()
        {
            RuleFor(x => x.AuditLogId)
                .NotEmpty()
                .WithMessage("Audit Log Id is required")
                .WithErrorCode("AUDITLOG.AUDITLOGID_REQUIRED");

            RuleFor(x => x.TenantId)
                .NotNull()
                .WithMessage("Tenant Id is required")
                .WithErrorCode("AUDITLOG.TENANTID_REQUIRED")
                ;

            RuleFor(x => x.EntityType)
                .NotEmpty()
                .WithMessage("Entity Type is required")
                .WithErrorCode("AUDITLOG.ENTITYTYPE_REQUIRED")
                .MaximumLength(100)
                .WithMessage("EntityType cannot exceed 100 characters")
                .WithErrorCode("AUDITLOG.ENTITYTYPE_LENGTH")
                ;

            RuleFor(x => x.EntityId)
                .NotNull()
                .WithMessage("Entity Id is required")
                .WithErrorCode("AUDITLOG.ENTITYID_REQUIRED")
                ;

            RuleFor(x => x.Action)
                .NotEmpty()
                .WithMessage("Action is required")
                .WithErrorCode("AUDITLOG.ACTION_REQUIRED")
                .MaximumLength(50)
                .WithMessage("Action cannot exceed 50 characters")
                .WithErrorCode("AUDITLOG.ACTION_LENGTH")
                ;

            RuleFor(x => x.UserId)
                .NotNull()
                .WithMessage("User Id is required")
                .WithErrorCode("AUDITLOG.USERID_REQUIRED")
                ;

            RuleFor(x => x.IPAddress)
                .MaximumLength(45)
                .WithMessage("IPAddress cannot exceed 45 characters")
                .WithErrorCode("AUDITLOG.IPADDRESS_LENGTH")
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$")
                .WithMessage("IPAddress has an invalid format")
                .WithErrorCode("AUDITLOG.IPADDRESS_FORMAT")
                ;

        }
    }
}
