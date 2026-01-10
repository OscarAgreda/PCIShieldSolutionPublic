using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
public static class SelectDataProviderHelper
{
    public static ISelectDataProvider<TDto, Guid> CreatePagedProvider<TDto>(
        Func<string, int, int, Task<(IEnumerable<TDto> items, int totalCount)>> fetchFunc,
        Func<TDto, Guid> getValue,
        Func<TDto, string> getDisplay)
    {
        return new EnhancedSelectDataProvider<TDto, Guid>(
            async (search, pageNumber, pageSize) =>
            {
                (IEnumerable<TDto> items, int totalCount) = await fetchFunc(search, pageNumber, pageSize);
                bool hasMore = totalCount > pageNumber * pageSize;
                return (items, hasMore);
            },
            getValue,
            getDisplay);
    }
}
public interface ISelectDataProvider<TEntity, TValue>
{
    Task<(IEnumerable<TEntity> Items, bool HasMore)> LoadItemsAsync(string search, int pageNumber, int pageSize);
    string GetDisplayText(TEntity item);
    TValue GetValue(TEntity item);
    RenderFragment<TEntity> ItemTemplate { get; }
    TEntity InitialItem { get; }
}
public class EnhancedSelectDataProvider<TEntity, TValue> : ISelectDataProvider<TEntity, TValue>
{
    private readonly Func<string, int, int, Task<(IEnumerable<TEntity> Items, bool HasMore)>> _loadDataAsync;
    private readonly TEntity _initialItem;
    private readonly bool _hasInitialItem;
    private readonly Func<TEntity, string> _getDisplayText;
    private readonly Func<TEntity, TValue> _getValue;
    private readonly RenderFragment<TEntity> _itemTemplate;
    private readonly ConcurrentDictionary<TValue, TEntity> _itemCache;
    public EnhancedSelectDataProvider(
        Func<string, int, int, Task<(IEnumerable<TEntity> Items, bool HasMore)>> loadDataAsync,
        Func<TEntity, TValue> getValue,
        Func<TEntity, string> getDisplayText,
        RenderFragment<TEntity> itemTemplate = null)
    {
        _loadDataAsync = loadDataAsync ?? throw new ArgumentNullException(nameof(loadDataAsync));
        _getDisplayText = getDisplayText ?? (x => x?.ToString() ?? string.Empty);
        _getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
        _itemTemplate = itemTemplate;
        _hasInitialItem = false;
        _itemCache = new ConcurrentDictionary<TValue, TEntity>();
    }
    public EnhancedSelectDataProvider(
        TEntity? initialItem, 
        Func<string, int, int, Task<(IEnumerable<TEntity> Items, bool HasMore)>> loadDataAsync,
        Func<TEntity, TValue> getValue,
        Func<TEntity, string> getDisplayText,
        RenderFragment<TEntity>? itemTemplate = null)
    {
        _initialItem = initialItem;
        _loadDataAsync = loadDataAsync ?? throw new ArgumentNullException(nameof(loadDataAsync));
        _getDisplayText = getDisplayText ?? (x => x?.ToString() ?? string.Empty);
        _getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
        _itemTemplate = itemTemplate;
        _hasInitialItem = initialItem != null;
        _itemCache = new ConcurrentDictionary<TValue, TEntity>();
        if (_initialItem != null)
        {
            _itemCache[_getValue(_initialItem)] = _initialItem;
        }
    }
    public async Task<(IEnumerable<TEntity> Items, bool HasMore)> LoadItemsAsync(
        string search, int pageNumber, int pageSize)
    {
        try
        {
            if (_hasInitialItem && _initialItem != null && pageNumber == 1)
            {
                var shouldIncludeInitial = string.IsNullOrWhiteSpace(search) ||
                                           _getDisplayText(_initialItem).Contains(search, StringComparison.OrdinalIgnoreCase);
                if (shouldIncludeInitial)
                {
                    if (!string.IsNullOrWhiteSpace(search) &&
                        _getDisplayText(_initialItem).Contains(search, StringComparison.OrdinalIgnoreCase))
                    {
                        return (new[] { _initialItem }, false);
                    }
                    pageSize = Math.Max(1, pageSize - 1);
                }
            }
            var (items, hasMore) = await _loadDataAsync(search, pageNumber, pageSize);
            var resultList = items.ToList();
            foreach (var item in resultList)
            {
                _itemCache[_getValue(item)] = item;
            }
            if (_hasInitialItem && pageNumber == 1)
            {
                var shouldIncludeInitial = string.IsNullOrWhiteSpace(search) ||
                    _getDisplayText(_initialItem).Contains(search, StringComparison.OrdinalIgnoreCase);
                var initialItemExists = resultList.Any(x =>
                    _getValue(x).Equals(_getValue(_initialItem)));
                if (shouldIncludeInitial && !initialItemExists)
                {
                    resultList.Insert(0, _initialItem);
                }
            }
            return (resultList, hasMore);
        }
        catch (Exception ex)
        {
            return (_hasInitialItem ? new[] { _initialItem } : Array.Empty<TEntity>(), false);
        }
    }
    public string GetDisplayText(TEntity item) => _getDisplayText(item);
    public TValue GetValue(TEntity item) => _getValue(item);
    public RenderFragment<TEntity> ItemTemplate => _itemTemplate;
    public TEntity InitialItem => _initialItem;
    public TEntity GetCachedItem(TValue value)
    {
        return _itemCache.TryGetValue(value, out var item) ? item : default;
    }
    public bool IsCached(TValue value) => _itemCache.ContainsKey(value);
}