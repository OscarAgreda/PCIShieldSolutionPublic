using System.Threading.Tasks;
namespace PCIShield.Domain.Interfaces
{
    public interface ITokenClaimsService
    {
        Task<string> GetTokenAsync(string userName);
    }
}