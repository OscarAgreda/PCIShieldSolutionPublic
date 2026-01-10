using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Fallback;
using Polly.Wrap;
namespace PCIShield.Infrastructure.Services
{
    public class PollyService : IPollyService
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly AsyncFallbackPolicy _fallbackPolicy;
        private readonly AsyncPolicyWrap _policyWrap;
        private readonly IAppLoggerService<PollyService> _logger;
        public PollyService(IAppLoggerService<PollyService> logger)
        {
            _logger = logger;
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    2,
                    retryAttempt => TimeSpan.FromSeconds(10),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                        _logger.LogWarning(
                            $"Delaying for {timeSpan.TotalSeconds} seconds, then making retry {retryCount}. Due to: {exception}"
                        )
                );
            _circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (exception, breakDelay) =>
                        _logger.LogWarning(
                            $"Circuit breaker opened for {breakDelay.TotalSeconds} seconds due to: {exception}"
                        ),
                    onReset: () => _logger.LogInformation("Circuit breaker reset."),
                    onHalfOpen: () =>
                        _logger.LogInformation(
                            "Circuit breaker half-open. Next successful operation will close it."
                        )
                );
            _fallbackPolicy = Policy
                .Handle<Exception>()
                .FallbackAsync(
                    async cancellationToken =>
                        _logger.LogWarning(
                            "Fallback action executing due to unsuccessful operation."
                        )
                );
            _policyWrap = Policy.WrapAsync(_fallbackPolicy, _retryPolicy, _circuitBreakerPolicy);
        }
        public async Task ExecuteWithPoliciesAsync(Func<Task> operation, CancellationToken cancellationToken)
        {
            await _policyWrap.ExecuteAsync(operation);
        }
        public void ExecuteActionWithRetryPolicy(Action operation, int retryCount, TimeSpan delay, CancellationToken cancellationToken)
        {
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(retryCount, _ => delay);
            retryPolicy.Execute(operation);
        }
        public TResult ExecuteFuncWithRetryPolicy<TResult>(Func<TResult> operation, int retryCount, TimeSpan delay, CancellationToken cancellationToken)
        {
            var retryPolicy = Policy.Handle<Exception>().Retry(retryCount);
            return retryPolicy.Execute(operation);
        }
        public async Task<TResult> ExecuteFuncWithRetryPolicyAsync<TResult>(Func<Task<TResult>> operation, int retryCount, TimeSpan delay, CancellationToken cancellationToken)
        {
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(retryCount, _ => delay);
            return await retryPolicy.ExecuteAsync(operation);
        }
        public void ExecuteWithCircuitBreaker(Action operation, int exceptionsAllowedBeforeBreaking, CancellationToken cancellationToken)
        {
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(exceptionsAllowedBeforeBreaking, TimeSpan.FromMinutes(1));
            circuitBreakerPolicy.Execute(operation);
        }
        public TResult ExecuteWithCircuitBreaker<TResult>(Func<TResult> operation, int exceptionsAllowedBeforeBreaking, CancellationToken cancellationToken)
        {
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(exceptionsAllowedBeforeBreaking, TimeSpan.FromMinutes(1));
            return circuitBreakerPolicy.Execute(operation);
        }
    }
}
