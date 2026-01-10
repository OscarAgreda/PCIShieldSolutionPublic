using System.Collections.Concurrent;

namespace PCIShield.PerformanceTests.Infrastructure;

/// <summary>
/// Thread-safe context for sharing test data across steps
/// </summary>
public class TestDataContext
{
    private readonly ConcurrentDictionary<string, object> _data = new();
    private readonly ConcurrentDictionary<string, List<Guid>> _entityLists = new();

    public void Set(string key, object value)
    {
        _data[key] = value;
    }

    public T Get<T>(string key)
    {
        return _data.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : default!;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (_data.TryGetValue(key, out var objValue) && objValue is T typedValue)
        {
            value = typedValue;
            return true;
        }
        value = default!;
        return false;
    }

    public void AddToList(string listKey, Guid id)
    {
        _entityLists.AddOrUpdate(listKey,
            new List<Guid> { id },
            (key, list) =>
            {
                list.Add(id);
                return list;
            });
    }

    public List<Guid> GetList(string listKey)
    {
        return _entityLists.TryGetValue(listKey, out var list)
            ? new List<Guid>(list)
            : new List<Guid>();
    }

    public Guid GetRandomFromList(string listKey, Random random)
    {
        var list = GetList(listKey);
        return list.Any() ? list[random.Next(list.Count)] : Guid.Empty;
    }

    public void Clear()
    {
        _data.Clear();
        _entityLists.Clear();
    }
}
