using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace PCIShield.PerformanceTests.Infrastructure
{
    /// <summary>
    /// Test-only in-memory implementation of ILocalStorageService for headless runners (NBomber).
    /// Thread-safe and JSON-compatible with Blazored.LocalStorage semantics.
    /// </summary>
    public sealed class InMemoryLocalStorageService : ILocalStorageService, IDisposable
    {
        private readonly ConcurrentDictionary<string, string> _store = new(StringComparer.Ordinal);

        public ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (_store.TryGetValue(key, out var raw))
            {
                if (typeof(T) == typeof(string))
                    return ValueTask.FromResult((T?)(object?)raw);

                var value = JsonSerializer.Deserialize<T>(raw);
                return ValueTask.FromResult(value);
            }
            return ValueTask.FromResult(default(T));
        }

        public ValueTask<string?> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(_store.TryGetValue(key, out var raw) ? raw : null);

        public ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default)
        {
            if (data is null)
            {
                _store.TryRemove(key, out _);
                return ValueTask.CompletedTask;
            }

            if (data is string s)
            {
                _store[key] = s;
            }
            else
            {
                _store[key] = JsonSerializer.Serialize(data);
            }
            return ValueTask.CompletedTask;
        }

        public ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default)
        {
            if (data is null)
            {
                _store.TryRemove(key, out _);
            }
            else
            {
                _store[key] = data;
            }
            return ValueTask.CompletedTask;
        }

        public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default)
        {
            _store.TryRemove(key, out _);
            return ValueTask.CompletedTask;
        }

        public ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            foreach (var key in keys)
                _store.TryRemove(key, out _);
            return ValueTask.CompletedTask;
        }

        public ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            _store.Clear();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(_store.ContainsKey(key));

        public ValueTask<int> LengthAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult(_store.Count);

        public ValueTask<string?> KeyAsync(int index, CancellationToken cancellationToken = default)
        {
            if (index < 0 || index >= _store.Count)
                return ValueTask.FromResult<string?>(null);

            var key = _store.Keys.ElementAt(index);
            return ValueTask.FromResult<string?>(key);
        }

        public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult<IEnumerable<string>>(_store.Keys.ToArray());

        // Required by the interface; no-ops in headless tests.
        public event EventHandler<ChangingEventArgs>? Changing;
        public event EventHandler<ChangedEventArgs>? Changed;

        public void Dispose() => _store.Clear();
    }
}

