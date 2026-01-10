using FluentValidation;
using System;
using System.Linq;
using System.Net.Mail;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.BlazorMauiShared.Models.Logs
{
    public class UpdateLogsRequest : BaseRequest
    {
        public LogsDto Logs { get; set; }
    }

    public class UpdateLogsValidator : AbstractValidator<LogsDto>
    {
        public UpdateLogsValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required")
                .WithErrorCode("LOGS.ID_REQUIRED");

        }
    }
}
