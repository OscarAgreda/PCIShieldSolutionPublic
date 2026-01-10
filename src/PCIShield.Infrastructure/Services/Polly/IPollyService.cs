using System;
using System.Threading;
using System.Threading.Tasks;
namespace PCIShield.Infrastructure.Services
{
    public interface IPollyService
    {
        Task ExecuteWithPoliciesAsync(Func<Task> operation, CancellationToken cancellationToken);
        void ExecuteActionWithRetryPolicy(Action operation, int retryCount, TimeSpan delay, CancellationToken cancellationToken);
        TResult ExecuteFuncWithRetryPolicy<TResult>(Func<TResult> operation, int retryCount, TimeSpan delay, CancellationToken cancellationToken);
        Task<TResult> ExecuteFuncWithRetryPolicyAsync<TResult>(Func<Task<TResult>> operation, int retryCount, TimeSpan delay, CancellationToken cancellationToken);
        void ExecuteWithCircuitBreaker(Action operation, int exceptionsAllowedBeforeBreaking, CancellationToken cancellationToken);
        TResult ExecuteWithCircuitBreaker<TResult>(Func<TResult> operation, int exceptionsAllowedBeforeBreaking, CancellationToken cancellationToken);
    }
}