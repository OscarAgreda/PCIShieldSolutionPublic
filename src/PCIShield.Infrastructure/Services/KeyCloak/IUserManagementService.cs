namespace PCIShield.Infrastructure.Services
{
    public interface IUserManagementService
    {
        string GetUserId();
        string GetJwtToken();
    }
}