using System;
namespace PCIShield.Domain.Interfaces
{
    public interface IApplicationSettings
    {
        int ClinicId { get; }
        DateTimeOffset TestDate { get; }
    }
}