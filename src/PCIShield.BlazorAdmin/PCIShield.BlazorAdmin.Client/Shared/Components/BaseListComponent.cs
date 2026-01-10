using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using PCIShield.BlazorAdmin.Client.SignalR;
using PCIShield.BlazorMauiShared.CustomDto;
using PCIShield.Domain.ModelsDto;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;
namespace PCIShield.BlazorAdmin.Client.Shared.Components
{
    public class PagedResult<TDto>
    {
        public List<TDto>? Items { get; set; } = new();
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
    public class GenericListOrchestrator<TDto> : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISnackbar _snackbar;
        private readonly IDialogService _dialog;
        private readonly ISignalRNotificationStrategy _signalR;
        private readonly IStringLocalizer _localizer;
        private readonly string _aggregatorName;
        private readonly CompositeDisposable _subscriptions = new();
        internal readonly SearchMetrics<TDto> _searchMetrics = new();
        public SearchMetrics<TDto> Analytics { get; } = new();
        private readonly Func<int, int, Task<PagedResult<TDto>>> _getAllPagedFunc;
        private readonly Func<int, int, Dictionary<string, string>, List<PCIShieldLib.SharedKernel.Interfaces.Sort>, Task<PagedResult<TDto>>> _getFilteredFunc;
        private readonly Action<TDto> _onEditNavigating;
        private readonly Action<TDto> _onViewNavigating;
        private readonly Action<TDto> _onDeleteAction;
        private readonly Subject<string> _searchInputSubject;
        private readonly Guid _editingUserId;
        private readonly Action<TDto, GenericSignalREvent> _onCreateEvent;
        private readonly Action<TDto, GenericSignalREvent> _onUpdateEvent;
        private readonly Action<TDto, GenericSignalREvent> _onDeleteEvent;
        private readonly Action<TDto, GenericSignalREvent> _onBeingEditedEvent;
        private readonly Action<TDto, GenericSignalREvent> _onEditingFinishedEvent;
        private GridState<TDto> _lastGridState = new();
        private readonly ISourceCache<TDto, string> _sourceCache;
        private readonly Subject<string> _searchSubject;
        private readonly IDisposable _searchSubscription;
        public IObservable<IChangeSet<TDto, string>> Connect() => _sourceCache.Connect();
        public bool IsBusy { get; private set; }
        public bool IsSearching { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public List<TDto>? Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool IsDataLoaded => Items.Any();
        private Action<string> _updateGridFilters;
        private Func<string, Func<TDto, bool>> _predicateFactory;
        private Func<Task> _reloadGridData;
        private readonly Func<Func<Task>, Task> _invokeAsync;
        private readonly Dictionary<string, SearchCacheEntry<TDto>> _searchCache = new();
        private readonly int _cacheSizeLimit = 100;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
        public bool Disposed { get; private set; }
        public GenericListOrchestrator(
            ILogger logger,
            ISnackbar snackbar,
            IDialogService dialog,
            ISignalRNotificationStrategy signalR,
            IStringLocalizer localizer,
            string aggregatorName,
            Func<int, int, Task<PagedResult<TDto>>> getAllPagedFunc,
            Func<int, int, Dictionary<string, string>, List<PCIShieldLib.SharedKernel.Interfaces.Sort>, Task<PagedResult<TDto>>> getFilteredFunc,
            Action<TDto> onEditNavigating,
            Action<TDto> onViewNavigating,
            Action<TDto> onDeleteAction,
            Guid editingUserId,
            Func<Func<Task>, Task> invokeAsync,
            Action<TDto, GenericSignalREvent> onCreateEvt,
            Action<TDto, GenericSignalREvent> onUpdateEvt,
            Action<TDto, GenericSignalREvent> onDeleteEvt,
            Action<TDto, GenericSignalREvent> onBeingEditedEvt,
            Action<TDto, GenericSignalREvent> onEditingFinishedEvt
        )
        {
            _logger = logger;
            _snackbar = snackbar;
            _dialog = dialog;
            _signalR = signalR;
            _localizer = localizer;
            _aggregatorName = aggregatorName;
            _getAllPagedFunc = getAllPagedFunc;
            _getFilteredFunc = getFilteredFunc;
            _onEditNavigating = onEditNavigating;
            _onViewNavigating = onViewNavigating;
            _onDeleteAction = onDeleteAction;
            _editingUserId = editingUserId;
            _onCreateEvent = onCreateEvt;
            _onUpdateEvent = onUpdateEvt;
            _onDeleteEvent = onDeleteEvt;
            _onBeingEditedEvent = onBeingEditedEvt;
            _onEditingFinishedEvent = onEditingFinishedEvt;
            _searchInputSubject = new Subject<string>();
            _invokeAsync = invokeAsync;
            InitializeSearchPipeline();
        }
        public void SetSearchDependencies(
            Action<string> updateGridFilters,
            Func<string, Func<TDto, bool>> predicateFactory,
            Func<Task> reloadGridData
        )
        {
            _updateGridFilters = updateGridFilters;
            _predicateFactory = predicateFactory;
            _reloadGridData = reloadGridData;
        }
        private void InitializeSearchPipeline()
        {
            var searchPipeline = _searchInputSubject
                .Throttle(TimeSpan.FromMilliseconds(400))
                .DistinctUntilChanged()
                .Where(term => string.IsNullOrEmpty(term) || term.Length >= 2);
            _subscriptions.Add(
                searchPipeline.Subscribe(term =>
                {
                    _invokeAsync(async () =>
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(term) && term.Length < 2)
                            {
                                _snackbar.Add(
                                    "Please enter at least 2 characters to search",
                                    Severity.Info
                                );
                                return;
                            }
                            _updateGridFilters?.Invoke(term);
                            var predicate = _predicateFactory?.Invoke(term);
                            await PerformSearch(
                                term,
                                async () =>
                                    await ServerReload(
                                        _lastGridState,
                                        new Dictionary<string, string>(),
                                        new List<PCIShieldLib.SharedKernel.Interfaces.Sort>()
                                    ),
                                async (searchTerm) =>
                                {
                                    var filterParams = new Dictionary<string, string>
                                    {
                                        { "search", searchTerm },
                                    };
                                    return await ServerReload(
                                        _lastGridState,
                                        filterParams,
                                        new List<PCIShieldLib.SharedKernel.Interfaces.Sort>()
                                    );
                                },
                                (searchTerm) =>
                                {
                                    if (Items != null && Items.Any() && predicate != null)
                                    {
                                        return Items.Where(predicate).ToList();
                                    }
                                    return null;
                                }
                            );
                            if (_reloadGridData != null)
                            {
                                await _reloadGridData();
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleError("Error in search pipeline", ex);
                        }
                    });
                })
            );
        }
        public void InitiateSearch(string term)
        {
            _searchInputSubject.OnNext(term);
        }
        public async Task InitializeAsync()
        {
            await _signalR.EnsureConnectedAsync();
            _subscriptions.Add(
                _signalR
                    .GetEventStream()
                    .Where(e => e.AggregateName == _aggregatorName)
                    .Where(e => e.UserId != _editingUserId.ToString())
                    .Buffer(TimeSpan.FromMilliseconds(100))
                    .Where(batch => batch.Any())
                    .Select(events =>
                        Observable.FromAsync(async () =>
                        {
                            foreach (var evt in events)
                            {
                                await HandleSignalREvent(evt);
                            }
                        })
                    )
                    .Concat()
                    .Subscribe(
                        onNext: _ => { },
                        onError: ex => _logger.LogError(ex, "Error handling SignalR events")
                    )
            );
        }
        public delegate Task<IEnumerable<TDto>> CustomSearchStrategy<T>(
            string searchTerm,
            IEnumerable<T> items,
            CancellationToken ct
        );
        private void ApplyClientSideSorting(GridState<TDto> state)
        {
            if (Items != null)
            {
                var orderedItems = Items.AsQueryable();
                foreach (var sort in state.SortDefinitions)
                {
                    var prop = typeof(TDto).GetProperty(sort.SortBy);
                    if (prop == null)
                        continue;
                    var parameter = Expression.Parameter(typeof(TDto), "x");
                    var property = Expression.Property(parameter, prop);
                    var lambda = Expression.Lambda(property, parameter);
                    var methodName = sort.Descending ? "OrderByDescending" : "OrderBy";
                    if (sort != state.SortDefinitions.First())
                        methodName = sort.Descending ? "ThenByDescending" : "ThenBy";
                    var orderByMethod = typeof(Queryable)
                        .GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(TDto), prop.PropertyType);
                    if (orderByMethod != null)
                    {
                        orderedItems =
                            orderByMethod.Invoke(null, [orderedItems, lambda]) as IQueryable<TDto>;
                    }
                }
                if (orderedItems != null)
                {
                    Items = orderedItems.ToList();
                }
            }
        }
        public async Task<GridData<TDto>> ServerReload(
            GridState<TDto> state,
            Dictionary<string, string> filterParams,
            List<PCIShieldLib.SharedKernel.Interfaces.Sort> sortParams
        )
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                SetBusy(true);
                bool doFiltering = filterParams.Any(kvp => !string.IsNullOrWhiteSpace(kvp.Value));
                PagedResult<TDto> result;
                if (!doFiltering && !sortParams.Any())
                {
                    result = await _getAllPagedFunc(state.Page + 1, state.PageSize);
                }
                else
                {
                    if (sortParams.Any())
                    {
                        var sort = sortParams.FirstOrDefault();
                        if (sort != null)
                        {
                            var a = sort.Field;
                            var b = sort.Direction;
                            var sortParamsList = new List<PCIShieldLib.SharedKernel.Interfaces.Sort>
                            {
                                new()
                                {
                                    Field = sort.Field,
                                    Direction = sort.Direction,
                                },
                            };
                            var testDebugValues = sortParamsList;
                        }
                    }
                    result = await _getFilteredFunc(
                        state.Page + 1,
                        state.PageSize,
                        filterParams,
                        sortParams
                    );
                }
                Items = result.Items ?? new List<TDto>();
                TotalCount = result.TotalCount;
                return new GridData<TDto> { Items = Items, TotalItems = TotalCount };
            }
            catch (Exception ex)
            {
                HandleError("Error reloading data", ex);
                return new GridData<TDto> { Items = new List<TDto>(), TotalItems = 0 };
            }
            finally
            {
                stopwatch.Stop();
                Analytics.RecordServerCall(stopwatch.Elapsed);
                bool hasSearchTerm =
                    filterParams.ContainsKey("search")
                    && !string.IsNullOrEmpty(filterParams["search"]);
                if (hasSearchTerm)
                {
                    TimeSpan searchDuration = stopwatch.Elapsed;
                    Analytics.RecordSearch(
                        filterParams["search"],
                        searchDuration,
                        isCacheHit: false
                    );
                }
                SetBusy(false);
            }
        }
        public async Task PerformSearch(
            string searchTerm,
            Func<Task<GridData<TDto>>> noSearchFunc,
            Func<string, Task<GridData<TDto>>> serverSearchFunc,
            Func<string, List<TDto>?> clientSearchFunc
        )
        {
            if (IsSearching)
                return;
            IsSearching = true;
            var searchStart = DateTime.UtcNow;
            var isCacheHit = false;
            try
            {
                SetBusy(true);
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    Analytics.RecordPartialSearch(searchTerm, TimeSpan.Zero);
                }
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    var res = await noSearchFunc();
                    Items = new List<TDto>(res.Items);
                    TotalCount = res.TotalItems;
                }
                else if (TryGetCache(searchTerm, out var cachedResults))
                {
                    Items = cachedResults;
                    if (cachedResults != null)
                    {
                        isCacheHit = true;
                        TotalCount = cachedResults.Count;
                        Analytics.RecordSearch(
                            searchTerm,
                            DateTime.UtcNow - searchStart,
                            isCacheHit: true
                        );
                        Analytics.CurrentCacheSize = _searchCache.Count;
                    }
                }
                else
                {
                    Analytics.RecordSearch(
                        searchTerm,
                        DateTime.UtcNow - searchStart,
                        isCacheHit: false
                    );
                    Analytics.CurrentCacheSize = _searchCache.Count;
                    var localResults = clientSearchFunc?.Invoke(searchTerm);
                    if (localResults != null && localResults.Any())
                    {
                        Items = localResults;
                        TotalCount = localResults.Count;
                        AddCache(searchTerm, Items);
                    }
                    else
                    {
                        var serverData = await serverSearchFunc(searchTerm);
                        Items = new List<TDto>(serverData.Items);
                        TotalCount = serverData.TotalItems;
                        AddCache(searchTerm, Items);
                    }
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        var searchDuration = DateTime.UtcNow - searchStart;
                        var lastPartial = Analytics.PartialSearches.LastOrDefault();
                        if (lastPartial != null && lastPartial.Term == searchTerm)
                        {
                            lastPartial.Duration = searchDuration;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("Error in PerformSearch", ex);
            }
            finally
            {
                IsSearching = false;
                SetBusy(false);
            }
        }
        private bool TryGetCache(string term, out List<TDto>? results)
        {
            results = null;
            if (string.IsNullOrWhiteSpace(term))
                return false;
            if (!_searchCache.TryGetValue(term, out var entry))
                return false;
            if (entry.IsExpired(_cacheExpiration))
            {
                _searchCache.Remove(term);
                return false;
            }
            results = entry.Results;
            return true;
        }
        private void AddCache(string term, List<TDto>? data)
        {
            if (string.IsNullOrEmpty(term))
                return;
            if (_searchCache.Count >= _cacheSizeLimit)
            {
                var oldest = _searchCache.OrderBy(x => x.Value.Timestamp).First();
                _searchCache.Remove(oldest.Key);
            }
            _searchCache[term] = new SearchCacheEntry<TDto>(data);
        }
        private async Task HandleSignalREvent(GenericSignalREvent evt)
        {
            switch (evt.Operation)
            {
                case SignalROperations.Create:
                    await OnCreate(evt);
                    break;
                case SignalROperations.Update:
                    await OnUpdate(evt);
                    break;
                case SignalROperations.Delete:
                    await OnDelete(evt);
                    break;
                case SignalROperations.BeingEdited:
                    BeingEdited(evt);
                    break;
                case SignalROperations.EditingFinished:
                    await EditingFinished(evt);
                    break;
                default:
                    _logger.LogInformation(
                        "Unhandled {Agg} operation: {Op}",
                        _aggregatorName,
                        evt.Operation
                    );
                    break;
            }
        }
        private async Task OnCreate(GenericSignalREvent evt)
        {
            var placeholder = default(TDto);
            if (placeholder != null)
            {
                _onCreateEvent?.Invoke(placeholder, evt);
            }
            await Task.CompletedTask;
        }
        private async Task OnUpdate(GenericSignalREvent evt)
        {
            var placeholder = default(TDto);
            if (placeholder != null)
            {
                _onUpdateEvent?.Invoke(placeholder, evt);
            }
            await Task.CompletedTask;
        }
        private async Task OnDelete(GenericSignalREvent evt)
        {
            var placeholder = default(TDto);
            if (placeholder != null)
            {
                _onDeleteEvent?.Invoke(placeholder, evt);
            }
            await Task.CompletedTask;
        }
        private void BeingEdited(GenericSignalREvent evt)
        {
            var placeholder = default(TDto);
            _onBeingEditedEvent?.Invoke(placeholder, evt);
        }
        private async Task EditingFinished(GenericSignalREvent evt)
        {
            var placeholder = default(TDto);
            if (placeholder != null)
            {
                _onEditingFinishedEvent?.Invoke(placeholder, evt);
            }
            await Task.CompletedTask;
        }
        public void OnEdit(TDto item)
        {
            _onEditNavigating?.Invoke(item);
        }
        public void OnView(TDto item)
        {
            _onViewNavigating?.Invoke(item);
        }
        public void OnDelete(TDto item)
        {
            _onDeleteAction?.Invoke(item);
        }
        public void SetBusy(bool busy)
        {
            if (IsBusy == busy)
                return;
            IsBusy = busy;
        }
        public void HandleError(string message, Exception ex)
        {
            var errorDetails = ex switch
            {
                HttpRequestException httpEx => $"API Error: {httpEx.StatusCode}",
                OperationCanceledException => "Operation was canceled",
                JsonException jsonEx => $"Data format error: {jsonEx.Path}",
                _ => ex.Message,
            };
            var full = $"{message}: {errorDetails}";
            _logger.LogError(ex, full);
            ErrorMessage = full;
            _snackbar.Add(full, Severity.Error);
        }
        private void TrimCache()
        {
            var oldEntries = _searchCache
                .Where(x => x.Value.IsExpired(_cacheExpiration))
                .Select(x => x.Key)
                .ToList();
            foreach (var key in oldEntries)
            {
                _searchCache.Remove(key);
            }
        }
        public void ClearError()
        {
            ErrorMessage = string.Empty;
        }
        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;
            _searchSubscription?.Dispose();
            _searchInputSubject?.Dispose();
            _subscriptions.Dispose();
            Items?.Clear();
            _searchCache.Clear();
            TrimCache();
        }
    }
    public class SearchCacheEntry<T>(List<T>? data)
    {
        public List<T>? Results { get; set; } = data;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsExpired(TimeSpan limit)
        {
            return (DateTime.UtcNow - Timestamp) > limit;
        }
    }
    public enum SearchType
    {
        Cache,
        Client,
        Server,
    }
    public class SearchMetrics<T>
    {
        public int TotalSearches { get; private set; }
        public int CacheHits { get; private set; }
        public TimeSpan AverageSearchTime { get; private set; }
        public Dictionary<string, int> PopularSearchTerms { get; } = new();
        public List<PartialSearchInfo> PartialSearches { get; } = new();
        public int CurrentCacheSize { get; set; }
        public int TotalServerCalls { get; private set; }
        public TimeSpan LastServerCallTime { get; private set; }
        public TimeSpan AverageServerCallTime { get; private set; }
        private readonly List<DateTime> _callTimestamps = new();
        public void RecordSearch(string term, TimeSpan duration, bool isCacheHit = false)
        {
            TotalSearches++;
            if (isCacheHit)
                CacheHits++;
            AverageSearchTime = TimeSpan.FromTicks(
                (AverageSearchTime.Ticks * (TotalSearches - 1) + duration.Ticks) / TotalSearches
            );
            if (!string.IsNullOrWhiteSpace(term))
            {
                if (!PopularSearchTerms.ContainsKey(term))
                    PopularSearchTerms[term] = 0;
                PopularSearchTerms[term]++;
            }
        }
        public IEnumerable<SearchTermAnalytics> GetTopSearchTerms(int take = 5)
        {
            if (!PopularSearchTerms.Any())
                return Enumerable.Empty<SearchTermAnalytics>();
            var total = PopularSearchTerms.Values.Sum();
            return PopularSearchTerms
                .OrderByDescending(x => x.Value)
                .Take(take)
                .Select(x => new SearchTermAnalytics
                {
                    Term = x.Key,
                    Count = x.Value,
                    Percentage = (x.Value * 100.0) / total,
                });
        }
        public void RecordPartialSearch(string partialTerm, TimeSpan duration)
        {
            PartialSearches.Add(
                new PartialSearchInfo
                {
                    Term = partialTerm,
                    Timestamp = DateTime.UtcNow,
                    Duration = duration,
                }
            );
            if (PartialSearches.Count > 50)
                PartialSearches.RemoveAt(0);
        }
        public void RecordServerCall(TimeSpan duration)
        {
            TotalServerCalls++;
            LastServerCallTime = duration;
            AverageServerCallTime = TimeSpan.FromTicks(
                (AverageServerCallTime.Ticks * (TotalServerCalls - 1) + duration.Ticks)
                    / TotalServerCalls
            );
            var now = DateTime.UtcNow;
            _callTimestamps.Add(now);
            _callTimestamps.RemoveAll(t => t < now.AddSeconds(-3));
        }
        public double[] GetCachePerformanceData()
        {
            if (TotalSearches == 0)
                return new[] { 0.0, 0.0 };
            double hits = CacheHits;
            double misses = TotalSearches - hits;
            return new[] { (hits * 100.0) / TotalSearches, (misses * 100.0) / TotalSearches };
        }
    }
    public class PartialSearchInfo
    {
        public string Term { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public class SearchTermAnalytics
    {
        public string Term { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
