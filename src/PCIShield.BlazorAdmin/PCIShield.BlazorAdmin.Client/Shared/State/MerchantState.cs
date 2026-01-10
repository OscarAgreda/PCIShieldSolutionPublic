using System.Collections.Concurrent;
using PCIShield.Domain.ModelsDto;
namespace PCIShield.BlazorAdmin.Client.Shared.State
{
    public class MerchantState
    {
        private static readonly ConcurrentDictionary<string, List<Action>> _listeners = new();
        private readonly ConcurrentDictionary<string, object> _state = new();
        private MerchantDto _merchant;
        public event Action<string> OnStateChange;
        public bool HasStateChanged { get; private set; }
        public static void Subscribe(string key, Action callback)
        {
            _listeners.AddOrUpdate(
                key,
                new List<Action> { callback },
                (_, list) => { list.Add(callback); return list; }
            );
        }
        public T? GetValue<T>(string key) =>
            _state.TryGetValue(key, out var value) ? (T)value : default;
        public void SetMerchant(MerchantDto merchant)
        {
            _merchant = merchant;
            HasStateChanged = true;
        }
        public void SetValue<T>(string key, T value)
        {
            _state.AddOrUpdate(key, value, (_, _) => value);
            NotifyStateChanged(key);
        }
        public void Unsubscribe(string key, Action callback)
        {
            if (_listeners.TryGetValue(key, out var list))
            {
                list.Remove(callback);
            }
        }
        public void UpdateMerchant(MerchantDto merchant)
        {
            _merchant = merchant;
            HasStateChanged = true;
        }
        private void NotifyStateChanged(string key)
        {
            if (_listeners.TryGetValue(key, out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    callback.Invoke();
                }
            }
            OnStateChange?.Invoke(key);
        }
    }
}