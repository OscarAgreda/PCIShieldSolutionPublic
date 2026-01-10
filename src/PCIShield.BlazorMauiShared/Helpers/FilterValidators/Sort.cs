using PCIShieldLib.SharedKernel.Interfaces;
namespace PCIShield.BlazorMauiShared.ModelsDto;
public class Sort : PCIShieldLib.SharedKernel.Interfaces.Sort
{
    public string Field { get; set; }
    public SortDirection Direction { get; set; }
}