using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class GetByIdROCPackageRequest : BaseRequest
    {
        public Guid AssessmentId { get; set; }
        public Guid ROCPackageId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

