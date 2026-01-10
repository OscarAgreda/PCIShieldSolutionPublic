using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.Logs
{
    public class DeleteLogsRequest : BaseRequest
    {
        public int Id { get; set; }
    }
}

