// ==========================================================

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using PCIShield.Client.Services.Merchant;
using PCIShield.Domain.ModelsDto;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace PCIShield.Infrastructure.Generics;
public interface IEntity<out TId>
{
    TId Id { get; }
    string Display { get; }
}
public static class EntityAdapter
{
    private static readonly Dictionary<Type, Func<object, IEntity<Guid>>> _registry = new();

    public static void Register<TDto>(
        Func<TDto, Guid> idSelector,
        Func<TDto, string> displaySelector)
        where TDto : class
        => _registry[typeof(TDto)] =
                dto => new EntityWrap<Guid>(idSelector((TDto)dto), displaySelector((TDto)dto));

    public static IEntity<Guid> AsEntity<TDto>(this TDto dto) where TDto : class
        => _registry.TryGetValue(typeof(TDto), out var adapt)
           ? adapt(dto)
           : throw new NotSupportedException($"{typeof(TDto).Name} is not registered");

    private sealed record EntityWrap<TId>(TId Id, string Display) : IEntity<TId>;
}
public interface IGeoNode
{
    Guid Id { get; }
    Guid? ParentId { get; }

    Task ApplyAndCascadeAsync(MerchantDto c, IHttpMerchantClientService svc);
}

public sealed record GeoNode<TDto>(
        Guid Id,
        Guid? ParentId,
        TDto Source,
        Action<MerchantDto, TDto> Apply,
        Action<MerchantDto> ClearChildren,
        Func<IHttpMerchantClientService, Guid,
             Task<IEnumerable<IGeoNode>>> LoadChildren)
    : IGeoNode
{
    public async Task ApplyAndCascadeAsync(MerchantDto c, IHttpMerchantClientService svc)
    {
        Apply(c, Source);
        ClearChildren(c);

        var children = await LoadChildren(svc, Id);
        foreach (var child in children)
            await child.ApplyAndCascadeAsync(c, svc);
    }
}
public static class Paged
{
    public interface IPage<out T>
    {
        IEnumerable<T> Items { get; }
        int Count { get; }
    }

    public static Task<IPage<T>> Fetch<T>(
        this IHttpMerchantClientService svc,
        int page,
        int size,
        string? search,
        Func<IHttpMerchantClientService, int, int, string?,
             Task<IPage<T>>> call)
        => call(svc, page, size, search);
}
public sealed class LookupProvider<TDto, TId> : ISelectDataProvider<TDto, TId>
    where TDto : class
{
    private readonly Func<int, int, string?, Task<(IEnumerable<TDto>, int)>> _fetch;
    private readonly TDto? _pinned;
    private readonly Func<TDto, TId> _id;
    private readonly Func<TDto, string> _display;

    public LookupProvider(
        Func<int, int, string?, Task<(IEnumerable<TDto>, int)>> fetch,
        TDto? pinned,
        Func<TDto, TId> id,
        Func<TDto, string> display)
    {
        _fetch = fetch;
        _pinned = pinned;
        _id = id;
        _display = display;
    }

    private async Task<(IEnumerable<TDto>, bool)> InternalGetItemsAsync(
        string? search, int page, int size)
    {
        var (data, total) = await _fetch(page, size, search);
        var list = data.ToList();

        if (page == 1 && _pinned is not null &&
            !list.Any(x => EqualityComparer<TId>.Default.Equals(_id(x), _id(_pinned))))
        {
            list.Insert(0, _pinned);
        }

        return (list, total > page * size);
    }
    public async Task<(IEnumerable<TDto> Items, bool HasMore)> LoadItemsAsync(
        string? search, int pageNumber, int pageSize)
        => await InternalGetItemsAsync(search, pageNumber, pageSize);

    public TId GetId(TDto item) => _id(item);
    public string GetDisplay(TDto item) => _display(item);
    public string GetDisplayText(TDto itm) => _display(itm);
    public TId GetValue(TDto itm) => _id(itm);
    public RenderFragment<TDto>? ItemTemplate { get; init; }
    public TDto? InitialItem { get; init; }
}
public sealed class JoinManager<TReq, TResp>
{
    private TReq? _pending;

    public void Queue(TReq? request) => _pending = request;
    public bool HasPending => _pending is not null;

    public async Task ProcessAsync(
        Func<TReq, Task<TResp>> send,
        Action<TResp> commit)
    {
        if (_pending is null) return;

        var response = await send(_pending);
        commit(response);
        _pending = default;
    }
}
public record UpsertSpec<TExisting, TReq, TResp>(
    Func<TExisting?> FindExisting,
    Func<TReq> BuildCreate,
    Func<TExisting, TReq> BuildUpdate,
    Func<TReq, Task<TResp>> Send,
    Action<TResp> Commit);

public static class Upsert
{
    public static async Task RunAsync<TExisting, TReq, TResp>(
        UpsertSpec<TExisting, TReq, TResp> spec)
    {
        var existing = spec.FindExisting();
        var request = existing is not null
                       ? spec.BuildUpdate(existing)
                       : spec.BuildCreate();

        var response = await spec.Send(request);
        spec.Commit(response);
    }
}
public static class EntityCollectionExtensions
{
    public static Task AddedTo<T>(
        this T entity,
        IList<T> collection,
        Subject<Unit> notify)
    {
        collection.Add(entity);
        notify.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public static Task UpdatedIn<T>(
        this T entity,
        IList<T> collection,
        Func<T, object> idSelector,
        Subject<Unit> notify)
    {
        var index = collection
                    .Select((item, idx) => new { item, idx })
                    .FirstOrDefault(x => Equals(idSelector(x.item), idSelector(entity)))
                    ?.idx ?? -1;

        if (index >= 0)
            collection[index] = entity;

        notify.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public static Task RemovedFrom<T>(
        this T entity,
        IList<T> collection,
        Func<T, object> idSelector,
        Subject<Unit> notify)
    {
        var index = collection
                    .Select((item, idx) => new { item, idx })
                    .FirstOrDefault(x => Equals(idSelector(x.item), idSelector(entity)))
                    ?.idx ?? -1;

        if (index >= 0)
            collection.RemoveAt(index);

        notify.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
}
