namespace PCIShield.BlazorMauiShared.CustomDto;

public class CommonMediatrFunctions
{
    public bool IsValidGuid(Guid? Id)
    {
        if (Id == null || Id == Guid.Empty)
        {
            return false;
        }
        return Guid.TryParse(Id.ToString(), out Guid guidOutputRCI) && guidOutputRCI != Guid.Empty;
    }
}