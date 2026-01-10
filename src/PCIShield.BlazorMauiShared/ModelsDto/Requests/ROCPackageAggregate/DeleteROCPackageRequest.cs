using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class DeleteROCPackageRequest : BaseRequest
    {
        public Guid ROCPackageId { get; set; }
        public Guid AssessmentId { get; set; }
    }
}

