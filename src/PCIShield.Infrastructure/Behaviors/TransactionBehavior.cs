/*
  I  have multiple ways to control transaction behavior:
   1. **Queries are automatically skipped**:
   ```csharp
   public class GetCustomerQuery : IRequest<CustomerDto>
   {
   }
   ```
   2. **Commands automatically get transactions**:
   ```csharp
   public class CreateCustomerCommand : IRequest<CustomerDto>
   {
   }
   ```
   3. **Explicit opt-in for special cases**:
   ```csharp
   public class SpecialOperation : IRequest<CustomerDto>, ITransactionalRequest
   {
   }
   ```
   4. **Explicit opt-out for commands**:
   ```csharp
   [SkipTransaction]
   public class BulkCustomerCommand : IRequest<CustomerDto>
   {
   }
   ```
   The behavior now makes decisions in this order:
   1. If it's a Query -> No transaction
   2. If it has `[SkipTransaction]` -> No transaction
   3. If it implements `ITransactionalRequest` -> Use transaction
   4. If it ends with "Command" -> Use transaction
   5. Otherwise -> No transaction
   This gives you:
   - Automatic transaction handling for standard commands
   - Easy opt-out for the 5% of commands that don't need transactions
   - No transactions for queries
   - Explicit control when needed through `ITransactionalRequest` and `[SkipTransaction]`
   This implementation differs slightly from your current repository-based transaction management because:
   1. It uses EF Core's transaction API directly, which is more reliable for complex scenarios
   2. It ensures proper async/await usage throughout
   3. It provides better logging and error tracking
   4. It operates at the DbContext level rather than the repository level
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using MediatR;
namespace PCIShield.Infrastructure.Behaviors
{
    public interface ITransactionalRequest { }
    [AttributeUsage(AttributeTargets.Class)]
    public class SkipTransactionAttribute : Attribute { }
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly AppDbContext _dbContext;
        private readonly IAppLoggerService<TransactionBehavior<TRequest, TResponse>> _logger;
        public TransactionBehavior(
            AppDbContext dbContext,
            IAppLoggerService<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (typeof(TRequest).Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
            {
                return await next();
            }
            if (Attribute.IsDefined(typeof(TRequest), typeof(SkipTransactionAttribute)))
            {
                return await next();
            }
            if (!typeof(TRequest).IsAssignableTo(typeof(ITransactionalRequest)) &&
                !typeof(TRequest).Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
            {
                return await next();
            }
            try
            {
                _logger.LogInformation($"Beginning transaction for {typeof(TRequest).Name}");
                await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var response = await next();
                    _logger.LogInformation($"Committing transaction for {typeof(TRequest).Name}");
                    await transaction.CommitAsync(cancellationToken);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during transaction for {typeof(TRequest).Name}. Rolling back.");
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling transaction for {typeof(TRequest).Name}");
                throw;
            }
        }
    }
}
