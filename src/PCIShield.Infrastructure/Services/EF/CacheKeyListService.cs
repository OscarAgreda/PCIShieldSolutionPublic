using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
namespace PCIShield.Infrastructure.Services
{
    public class CacheKeyListService : ICacheKeyListService
    {
        private readonly ConcurrentDictionary<string, byte> _keys = new ConcurrentDictionary<string, byte>();
        public void AddKey(string key)
        {
            _keys.TryAdd(key, 0);
        }
        public void RemoveKey(string key)
        {
            _keys.TryRemove(key, out byte removedValue);
        }
        public IEnumerable<string> GetKeys()
        {
            return _keys.Keys;
        }
    }
}
