using System;
using PCIShield.Domain.Interfaces;
namespace PCIShield.Api
{
    public class OfficeSettings : IApplicationSettings
    {
        public int ClinicId => 1;
        public DateTimeOffset TestDate => new(2030, 9, 23, 0, 0, 0, new TimeSpan(-4, 0, 0));
    }
}