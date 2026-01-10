using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ardalis.Specification;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IEntityFactory<T>
    {
        T Create(params object[] args);
    }
    public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
    {
        public void BeginTransaction();
        public void CommitTransaction();
        Task<T> FirstOrDefaultAsyncWithIncludeOptimized(ISpecification<T> spec, CancellationToken cancellationToken = default);
        public void RollbackTransaction();
        Task<PagedResult<T>> GetPagedResultAsync(
            PagedSpecification<T> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<PagedResult<TResult>> GetPagedResultAsync<TResult>(
            PagedSpecification<T, TResult> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
    public interface IPagedSpecification<T>
    {
        Specification<T> GetCountSpecification();
    }
    public abstract class PagedSpecification<T> : Specification<T>, IPagedSpecification<T>
    {
        protected PagedSpecification() : base()
        {
        }
        public virtual Specification<T> GetCountSpecification()
        {
            var countSpec = new CountSpecification<T>();
            foreach (var whereExpression in this.WhereExpressions)
            {
                ((List<WhereExpressionInfo<T>>)countSpec.WhereExpressions).Add(whereExpression);
            }
            return countSpec;
        }
    }
    public abstract class PagedSpecification<T, TResult> : Specification<T, TResult>, IPagedSpecification<T>
    {
        protected PagedSpecification() : base()
        {
        }
        public virtual Specification<T> GetCountSpecification()
        {
            var countSpec = new CountSpecification<T>();
            foreach (var whereExpression in this.WhereExpressions)
            {
                ((List<WhereExpressionInfo<T>>)countSpec.WhereExpressions).Add(whereExpression);
            }
            return countSpec;
        }
    }
    public class SimplePagedSpecification<T> : PagedSpecification<T> where T : class, IAggregateRoot
    {
        public SimplePagedSpecification(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));
            if (filter != null)
            {
                Query.Where(filter);
            }
            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
    public class CountSpecification<T> : Specification<T>
    {
        public CountSpecification() : base()
        {
        }
    }
    public static class RepositoryExtensions
    {
        public static async Task<PagedResult<T>> GetPagedResultAsync<T>(
            this IRepository<T> repository,
            PagedSpecification<T> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default) where T : class, IAggregateRoot
        {
            var countSpec = specification.GetCountSpecification();
            var itemsTask = repository.ListAsync(specification, cancellationToken);
            var countTask = repository.CountAsync(countSpec, cancellationToken);
            await Task.WhenAll(itemsTask, countTask);
            return new PagedResult<T>
            {
                Items = itemsTask.Result,
                TotalCount = countTask.Result,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public static async Task<PagedResult<TResult>> GetPagedResultAsync<T, TResult>(
            this IRepository<T> repository,
            PagedSpecification<T, TResult> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            var countSpec = specification.GetCountSpecification();
            var itemsTask = repository.ListAsync(specification, cancellationToken);
            var countTask = repository.CountAsync(countSpec, cancellationToken);
            await Task.WhenAll(itemsTask, countTask);
            return new PagedResult<TResult>
            {
                Items = itemsTask.Result,
                TotalCount = countTask.Result,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
    }