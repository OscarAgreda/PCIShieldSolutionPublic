namespace PCIShield.Domain.ModelsDto;
public record ComparisonError(
    string Field,
    object Expected,
    object Actual,
    string Message = null
);