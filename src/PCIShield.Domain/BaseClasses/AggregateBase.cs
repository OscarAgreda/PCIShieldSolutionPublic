using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.DataTypes.Serialisation;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;
using Unit = LanguageExt.Unit;
namespace PCIShield.Domain.BaseClasses
{
    public interface IBusinessRule<T> where T : IAggregateRoot
    {
        Either<Error, T> Apply(T entity);
        Validation<Error, Unit> Validate(T entity);
    }
    public interface ICurrency
    { }
    public interface IFilterCriteria
    {
        Dictionary<string, string> Filters { get; }
        List<Sort> Sorting { get; }
    }
    public interface IStateMachine<TState, TEvent>
            where TState : struct
            where TEvent : IBaseDomainEvent
    {
        bool CanTransition(TState from, TState to);
        Either<Error, TState> Transition(TState current, TEvent evt);
    }
    public abstract class AggregateCollection<TAggregate, TItem>
            where TAggregate : IAggregateRoot
            where TItem : EntityBase
    {
        protected readonly ImmutableList<TItem> _items;
        public Either<Error, Unit> Add(TItem item) =>
            ValidateItem(item)
                .Bind(valid => AddCore(valid))
                .Map(_ => RecalculateTotals());
        protected abstract Either<Error, Unit> AddCore(TItem item);
        protected abstract Unit RecalculateTotals();
        protected abstract Either<Error, TItem> ValidateItem(TItem item);
    }
    public abstract class AggregateRootBase<TId> : BaseEntityEv<TId>, IAggregateRoot
    {
        private ImmutableStack<AggregateRootBase<TId>> _snapshots = ImmutableStack<AggregateRootBase<TId>>.Empty;
        public Option<AggregateRootBase<TId>> RestoreSnapshot()
        {
            if (_snapshots.IsEmpty) return Option<AggregateRootBase<TId>>.None;
            _snapshots = _snapshots.Pop(out var prev);
            return Some(prev);
        }
        public void TakeSnapshot() => _snapshots = _snapshots.Push(this.MemberwiseClone() as AggregateRootBase<TId>);
        protected IEnumerable<EitherData<Error, Unit>> ApplyBusinessRules<TRule, TEntity>(TRule rule)
            where TRule : IBusinessRule<TEntity>
            where TEntity : AggregateRootBase<TId>
        {
            var castThis = this as TEntity;
            return castThis == null
                ? Enumerable.Repeat(EitherData.Left<Error, Unit>(Error.New("Invalid cast for ApplyBusinessRules")), 1)
                : rule.Validate(castThis)
                    .Bind(_ => rule.Apply(castThis).Map(__ => Unit.Default));
        }
    }
    public abstract class FilteredSpecification<TEntity, TEntityDto, TFilter> : ProjectingSpecification<TEntity, TEntityDto, TFilter>
        where TEntity : class, IAggregateRoot
        where TEntityDto : class
        where TFilter : class, IFilterCriteria
    {
        private readonly TFilter _criteria;
        protected FilteredSpecification(TFilter criteria)
        {
            _criteria = Guard.Against.Null(criteria, nameof(criteria));
            Query.Where(e => !EF.Property<bool>(e, "IsDeleted"));
            if (_criteria.Filters?.Any() == true)
                ApplyFilters(_criteria.Filters);
            if (_criteria.Sorting?.Any() == true)
                ConfigureOrdering(_criteria.Sorting);
        }
        protected abstract void ApplyFilter(string key, string value, ISpecificationBuilder<TEntity> query);
        protected virtual void ApplyFilters(Dictionary<string, string> filters)
        {
            foreach (var filter in filters.Where(f => !string.IsNullOrEmpty(f.Value)))
            {
                ApplyFilter(filter.Key.ToLower(), filter.Value, Query);
            }
        }
        protected override string GetCacheKey()
        {
            var filterKey = _criteria.Filters?.Any() == true
                ? string.Join("-", _criteria.Filters.Select(f => $"{f.Key}={f.Value}"))
                : "nofilters";
            var sortKey = _criteria.Sorting?.Any() == true
                ? string.Join("-", _criteria.Sorting.Select(s => $"{s.Field}={s.Direction}"))
                : "nosort";
            return $"{base.GetCacheKey()}-{filterKey}-{sortKey}";
        }
        protected override Expression<Func<TEntity, object>> GetSortProperty(string propertyName) =>
            entity => EF.Property<object>(entity, propertyName);
    }
    public abstract class MonetaryCalculator<T> where T : IAggregateRoot
    {
        public Either<Error, Money<ICurrency>> CalculateTotal(T entity, TaxRules rules)
        {
            var subtotal = CalculateSubtotal(entity);
            var taxes = CalculateTaxes(entity, rules);
            return Right<Error, Money<ICurrency>>(subtotal + taxes);
        }
        protected abstract Money<ICurrency> CalculateDiscounts(T entity);
        protected abstract Money<ICurrency> CalculateSubtotal(T entity);
        protected abstract Money<ICurrency> CalculateTaxes(T entity, TaxRules rules);
    }
    public class Money<T> where T : ICurrency
    {
        public Money(decimal amount, T currency)
        {
            Amount = amount;
            Currency = currency;
        }
        public decimal Amount { get; }
        public T Currency { get; }
        public static Money<T> operator +(Money<T> left, Money<T> right)
        {
            return new Money<T>(left.Amount + right.Amount, left.Currency);
        }
    }
    public sealed class PagedSpecification<TEntity, TEntityDto> : ProjectingSpecification<TEntity, TEntityDto, PagingCriteria>
        where TEntity : class, IAggregateRoot
        where TEntityDto : class
    {
        private readonly PagingCriteria _criteria;
        public PagedSpecification(PagingCriteria criteria)
        {
            _criteria = Guard.Against.Null(criteria, nameof(criteria));
            ConfigurePaging(criteria.PageNumber, criteria.PageSize);
        }
        protected override string GetCacheKey() =>
            $"{base.GetCacheKey()}-{_criteria.PageNumber}-{_criteria.PageSize}";
        protected override Expression<Func<TEntity, object>> GetSortProperty(string propertyName) =>
            entity => EF.Property<int>(entity, "Id");
    }
    public class PagingCriteria
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public abstract class ProjectingSpecification<TEntity, TEntityDto>
        : Specification<TEntity, TEntityDto>
        where TEntity : class, IAggregateRoot
        where TEntityDto : class
    {
        protected abstract Expression<Func<TEntity, TEntityDto>> ProjectionExpression { get; }
        protected virtual IQueryable<TEntityDto> ProjectToEntityDto(IQueryable<TEntity> query) =>
                    query.Select(ProjectionExpression);
    }
    public abstract class ProjectingSpecification<TEntity, TEntityDto, TCriteria> : Specification<TEntity, TEntityDto>
    where TEntity : class, IAggregateRoot
    where TEntityDto : class
    where TCriteria : class
    {
        protected ProjectingSpecification()
        {
            Query.AsNoTracking()
                .AsSplitQuery()
                .EnableCache(GetCacheKey());
        }
        protected virtual void ConfigureOrdering(IEnumerable<Sort> sorting)
        {
            if (sorting?.Any() != true)
                return;
            var first = sorting.First();
            var ordered = ApplySort(Query, first);
            foreach (var sort in sorting.Skip(1))
            {
                ordered = ApplyAdditionalSort(ordered, sort);
            }
        }
        protected virtual void ConfigurePaging(int pageNumber, int pageSize)
        {
            _ =  Guard.Against.Negative(pageNumber, nameof(pageNumber));
            _ =  Guard.Against.Negative(pageSize, nameof(pageSize));
            Query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
        protected virtual string GetCacheKey() =>
            $"{GetType().Name}-{typeof(TEntity).Name}-{typeof(TEntityDto).Name}";
        protected abstract Expression<Func<TEntity, object>> GetSortProperty(string propertyName);
        private IOrderedSpecificationBuilder<TEntity> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<TEntity> query,
            Sort sort)
        {
            var property = GetSortProperty(sort.Field);
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(property)
                : query.ThenBy(property);
        }
        private IOrderedSpecificationBuilder<TEntity> ApplySort(
                                    ISpecificationBuilder<TEntity> query,
            Sort sort)
        {
            var property = GetSortProperty(sort.Field);
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(property)
                : query.OrderBy(property);
        }
    }
    public abstract class SystemTypeBase : EntityBase
    {
        public string Code { get; protected set; }
        public string Description { get; protected set; }
        public bool IsActive { get; protected set; } = true;
        protected Either<Error, Unit> UpdateStatus(bool isActive)
        {
            if (isActive == IsActive) return Right<Error, Unit>(Unit.Default);
            IsActive = isActive;
            return Right<Error, Unit>(Unit.Default);
        }
    }
    public class TaxRules
    { }
    public abstract class TrackableAggregate<TId> : AggregateRootBase<TId>
    {
        protected Either<Error, Unit> ApplySystemTypeUpdate<TSystemType>(
            Guid currentId,
            Guid newId,
            Action<TrackableAggregate<TId>, string> eventRaiser)
            where TSystemType : SystemTypeBase
        {
            if (newId == currentId)
                return Unit.Default;
            eventRaiser(this, $"SystemType changed from {currentId} to {newId}");
            return Unit.Default;
        }
    }
    public abstract class ValidationHandler<T> where T : IAggregateRoot
    {
        public Validation<Error, T> Validate(T entity) =>
            from basic in ValidateBasicRules(entity)
            from business in ValidateBusinessRules(entity)
            from domain in ValidateDomainRules(entity)
            select entity;
        protected abstract Validation<Error, T> ValidateBasicRules(T entity);
        protected abstract Validation<Error, T> ValidateBusinessRules(T entity);
        protected abstract Validation<Error, T> ValidateDomainRules(T entity);
    }
}