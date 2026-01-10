using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Logs
{
    public class GetByIdLogsRequest : BaseRequest
    {
        public int Id { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

