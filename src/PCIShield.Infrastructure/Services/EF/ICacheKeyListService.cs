using System.Collections.Generic;
namespace PCIShield.Infrastructure.Services
{
    public interface ICacheKeyListService
    {
        void AddKey(string key);
        void RemoveKey(string key);
        IEnumerable<string> GetKeys();
    }
}