using FluentValidation;
using System;
using System.Linq;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;
namespace PCIShieldCore.BlazorMauiShared.CustomModelsDto
{
    public class UploadFileResponse : BaseResponse
    {
        public UploadFileResponse(Guid correlationId)
            : base(correlationId)
        {
        }
        public UploadFileResponse()
        {
        }
        public string ObjectIdStr { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
    }
}