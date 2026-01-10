using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ardalis.Specification;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Infrastructure.Services;
using PCIShield.Infrastructure.Services.Redis;
using PCIShieldLib.SharedKernel.Interfaces;

using LanguageExt;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;
using Array = System.Array;
using IRequest = MediatR.IRequest;

namespace PCIShield.Api.CQRS
{
    public interface ICommand<TResponse> { }
    public interface ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        Task<Either<string, TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
    public interface IQuery<TResponse> { }
    public interface IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<Either<string, TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
    public interface ICommandMediator
    {
        Task<Either<string, TResponse>> SendCommand<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
        Task<Either<string, TResponse>> SendQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    }
    public class CommandMediator : ICommandMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandMediator> _logger;
        public CommandMediator(IServiceProvider serviceProvider, ILogger<CommandMediator> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task<Either<string, TResponse>> SendCommand<TResponse>(
            ICommand<TResponse> command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Type handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
                if (!_serviceProvider.IsRegistered(handlerType))
                {
                    _logger.LogError($"No handler registered for command type {command.GetType().Name}");
                    return Left<string, TResponse>($"No handler registered for command type {command.GetType().Name}");
                }
                dynamic handler = _serviceProvider.GetRequiredService(handlerType);
                return await handler.HandleAsync((dynamic)command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing command {command.GetType().Name}");
                return Left<string, TResponse>($"Error executing command: {ex.Message}");
            }
        }
        public async Task<Either<string, TResponse>> SendQuery<TResponse>(
            IQuery<TResponse> query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
                if (!_serviceProvider.IsRegistered(handlerType))
                {
                    _logger.LogError($"No handler registered for query type {query.GetType().Name}");
                    return Left<string, TResponse>($"No handler registered for query type {query.GetType().Name}");
                }
                dynamic handler = _serviceProvider.GetRequiredService(handlerType);
                return await handler.HandleAsync((dynamic)query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing query {query.GetType().Name}");
                return Left<string, TResponse>($"Error executing query: {ex.Message}");
            }
        }
    }
    public static class ServiceProviderExtensions
    {
        public static bool IsRegistered(this IServiceProvider provider, Type serviceType)
        {
            try
            {
                var service = provider.GetService(serviceType);
                return service != null;
            }
            catch
            {
                return false;
            }
        }
    }
    public class EntityCacheInvalidationOptions<TEntity>
        where TEntity : class, IAggregateRoot
    {
        public Type? MainDtoType { get; set; }
        public IEnumerable<Type> AdditionalProjectionTypes { get; set; } = Array.Empty<Type>();
        public IEnumerable<string> IdBasedSpecifications { get; set; } = Array.Empty<string>();
        public IEnumerable<string> GeneralSpecifications { get; set; } = Array.Empty<string>();
        public Func<RedisCacheRepository<TEntity>, Guid?, int?, int?, Task>? CustomInvalidationAction { get; set; }
    }
    public static class CacheInvalidationExtensions
    {
        public static async Task InvalidateEntityCache<TEntity, TRepository>(
            this TRepository repository,
            Guid? entityId = null,
            int? pageSize = null,
            int? pageNumber = null,
            EntityCacheInvalidationOptions<TEntity>? options = null)
            where TEntity : class, IAggregateRoot
            where TRepository : RedisCacheRepository<TEntity>
        {
            options ??= new EntityCacheInvalidationOptions<TEntity>();
            await repository.InvalidateEntityCacheAsync<TEntity>(
                entityId: entityId,
                pageSize: pageSize,
                pageNumber: pageNumber,
                projectionType: options.MainDtoType);
            foreach (var projectionType in options.AdditionalProjectionTypes)
            {
                await repository.InvalidateEntityCacheAsync<TEntity>(
                    entityId: entityId,
                    pageSize: pageSize,
                    pageNumber: pageNumber,
                    projectionType: projectionType);
            }
            if (entityId.HasValue)
            {
                foreach (var specTemplate in options.IdBasedSpecifications)
                {
                    string specName = string.Format(specTemplate, entityId);
                    await repository.InvalidateEntityCacheAsync<TEntity>(
                        customSpecificationName: specName);
                }
            }
            foreach (var specName in options.GeneralSpecifications)
            {
                await repository.InvalidateEntityCacheAsync<TEntity>(
                    customSpecificationName: specName);
            }
            if (options.CustomInvalidationAction != null)
            {
                await options.CustomInvalidationAction(repository, entityId, pageSize, pageNumber);
            }
        }
    }
    public class MediatorAdapter : IMediator
    {
        private readonly IMediator _inner;
        private readonly ICommandMediator _commandMediator;
        public MediatorAdapter(IMediator inner, ICommandMediator commandMediator)
        {
            _inner = inner;
            _commandMediator = commandMediator;
        }
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return _inner.Send(request, cancellationToken);
        }
        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            return _inner.Send(request, cancellationToken);
        }
        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            return _inner.Send(request, cancellationToken);
        }
        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return _inner.Publish(notification, cancellationToken);
        }
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return _inner.Publish(notification, cancellationToken);
        }
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            return _inner.CreateStream(request, cancellationToken);
        }
        public IAsyncEnumerable<object?> CreateStream(
            object request,
            CancellationToken cancellationToken = default)
        {
            return _inner.CreateStream(request, cancellationToken);
        }
        public Task<Either<string, TResponse>> SendCommand<TResponse>(
            ICommand<TResponse> command,
            CancellationToken cancellationToken = default)
        {
            return _commandMediator.SendCommand(command, cancellationToken);
        }
        public Task<Either<string, TResponse>> SendQuery<TResponse>(
            IQuery<TResponse> query,
            CancellationToken cancellationToken = default)
        {
            return _commandMediator.SendQuery(query, cancellationToken);
        }
    }
    public class SpecificationPerformanceTracker<T, TDto>
        where T : class
        where TDto : class
    {
        private readonly Dictionary<string, List<long>> _timings = new();
        private readonly Dictionary<string, int> _propertyCountEstimates = new();
        private readonly Dictionary<string, int> _entityCountEstimates = new();
        internal Dictionary<string, SpecificationPerformanceResponse<TDto>.QueryComplexityAnalysis.SpecComplexityMetrics>
            _complexityMetrics = new();
        private readonly IAppLoggerService _logger;
        public SpecificationPerformanceTracker(IAppLoggerService logger)
        {
            _logger = logger;
        }
        public void ConfigureEntityMetrics(
            Dictionary<string, int> propertyCountEstimates,
            Dictionary<string, int> entityCountEstimates,
            Dictionary<string, SpecificationPerformanceResponse<TDto>.QueryComplexityAnalysis.SpecComplexityMetrics> complexityMetrics)
        {
            foreach (var pair in propertyCountEstimates)
                _propertyCountEstimates[pair.Key] = pair.Value;
            foreach (var pair in entityCountEstimates)
                _entityCountEstimates[pair.Key] = pair.Value;
            foreach (var pair in complexityMetrics)
                _complexityMetrics[pair.Key] = pair.Value;
        }
        public void AddTiming(string specName, long elapsedMs)
        {
            if (!_timings.ContainsKey(specName))
            {
                _timings[specName] = new List<long>();
            }
            _timings[specName].Add(elapsedMs);
            _logger.LogInformation($"{specName} execution time: {elapsedMs}ms");
        }
        public Dictionary<string, List<long>> GetTimings() => _timings;
        public SpecificationPerformanceResponse<TDto>.QueryComplexityAnalysis GetComplexityAnalysis()
        {
            var metricsCopy = new Dictionary<string, SpecificationPerformanceResponse<TDto>.QueryComplexityAnalysis.SpecComplexityMetrics>();
            foreach (var pair in _complexityMetrics)
            {
                metricsCopy[pair.Key] = new SpecificationPerformanceResponse<TDto>.QueryComplexityAnalysis.SpecComplexityMetrics
                {
                    EstimatedJoinCount = pair.Value.EstimatedJoinCount,
                    IncludeCount = pair.Value.IncludeCount,
                    ProjectionDepth = pair.Value.ProjectionDepth,
                    UsesSplitQuery = pair.Value.UsesSplitQuery,
                    UsesNoTracking = pair.Value.UsesNoTracking,
                    RelationshipDepth = pair.Value.RelationshipDepth,
                    ComplexityLevel = pair.Value.ComplexityLevel,
                    CalculatedPropertyCount = pair.Value.CalculatedPropertyCount
                };
            }
            return new SpecificationPerformanceResponse<TDto>.QueryComplexityAnalysis
            {
                SpecificationMetrics = metricsCopy
            };
        }
        public bool UpdateMetric<TMetric>(string specName, string metricName, TMetric value)
        {
            if (!_complexityMetrics.ContainsKey(specName))
                return false;
            var propertyInfo = typeof(SpecificationPerformanceResponse<TDto>.QueryComplexityAnalysis.SpecComplexityMetrics)
                .GetProperty(metricName);
            if (propertyInfo == null || !propertyInfo.CanWrite)
                return false;
            try
            {
                propertyInfo.SetValue(_complexityMetrics[specName], value);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void UpdateSpecificationComplexityMetrics<T, TDto>(
            ISpecification<T, TDto> spec,
            string specName,
            SpecificationPerformanceTracker<T, TDto> tracker)
            where T : class
            where TDto : class
        {
            if (!tracker._complexityMetrics.ContainsKey(specName))
                return;
            var metrics = tracker._complexityMetrics[specName];
            try
            {
                if (spec.GetType().GetProperty("Includes") != null)
                {
                    var includesProperty = spec.GetType().GetProperty("Includes");
                    if (includesProperty != null)
                    {
                        var includes = includesProperty.GetValue(spec) as System.Collections.IEnumerable;
                        if (includes != null)
                        {
                            int count = 0;
                            foreach (var _ in includes)
                                count++;
                            metrics.IncludeCount = count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            metrics.UsesSplitQuery =
                spec.GetType().GetMethod("AsSplitQuery") != null ||
                spec.ToString().Contains("SplitQuery");
            metrics.UsesNoTracking =
                spec.GetType().GetMethod("AsNoTracking") != null ||
                spec.ToString().Contains("NoTracking");
        }
        public Dictionary<string, SpecificationPerformanceResponse<TDto>.PerformanceStats> CalculateStatistics()
        {
            var stats = new Dictionary<string, SpecificationPerformanceResponse<TDto>.PerformanceStats>();
            long? fastestAverage = null;
            foreach (var entry in _timings)
            {
                var values = entry.Value;
                var count = values.Count;
                if (count == 0) continue;
                double average = values.Average();
                long min = values.Min();
                long max = values.Max();
                double sumOfSquares = values.Sum(v => Math.Pow(v - average, 2));
                double standardDeviation = Math.Sqrt(sumOfSquares / count);
                var sortedValues = values.OrderBy(v => v).ToList();
                double median = count % 2 == 0
                    ? (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2.0
                    : sortedValues[count / 2];
                int p95Index = (int)Math.Ceiling(count * 0.95) - 1;
                double p95 = sortedValues[Math.Min(p95Index, count - 1)];
                double rps = 1000.0 / average;
                double variabilityCoefficient = standardDeviation / average;
                string performanceCategory;
                if (average < 100)
                    performanceCategory = "Excellent";
                else if (average < 250)
                    performanceCategory = "Good";
                else if (average < 500)
                    performanceCategory = "Fair";
                else
                    performanceCategory = "Poor";
                stats[entry.Key] = new SpecificationPerformanceResponse<TDto>.PerformanceStats
                {
                    AverageMs = Math.Round(average, 2),
                    MinMs = min,
                    MaxMs = max,
                    RunCount = count,
                    StandardDeviationMs = Math.Round(standardDeviation, 2),
                    MedianMs = Math.Round(median, 2),
                    Percentile95Ms = Math.Round(p95, 2),
                    RelativePerformanceRatio = 0,
                    Rank = 0,
                    RequestsPerSecond = Math.Round(rps, 2),
                    VariabilityCoefficient = Math.Round(variabilityCoefficient, 2),
                    PerformanceCategory = performanceCategory,
                    EstimatedPropertyCount = _propertyCountEstimates.ContainsKey(entry.Key) ?
                        _propertyCountEstimates[entry.Key] : 0,
                    EstimatedEntityCount = _entityCountEstimates.ContainsKey(entry.Key) ?
                        _entityCountEstimates[entry.Key] : 0
                };
                if (!fastestAverage.HasValue || average < fastestAverage)
                {
                    fastestAverage = (long)average;
                }
            }
            if (fastestAverage.HasValue)
            {
                var orderedSpecs = stats.OrderBy(s => s.Value.AverageMs).ToList();
                for (int i = 0; i < orderedSpecs.Count; i++)
                {
                    var key = orderedSpecs[i].Key;
                    stats[key].Rank = i + 1;
                    stats[key].RelativePerformanceRatio = Math.Round(stats[key].AverageMs / fastestAverage.Value, 2);
                }
            }
            return stats;
        }
        public void LogSummary()
        {
            _logger.LogInformation("=== SPECIFICATION PERFORMANCE SUMMARY ===");
            var stats = CalculateStatistics();
            var sortedStats = stats.OrderBy(s => s.Value.Rank).ToList();
            _logger.LogInformation("=== PERFORMANCE RANKING ===");
            foreach (var entry in sortedStats)
            {
                var stat = entry.Value;
                _logger.LogInformation(
                    $"#{stat.Rank}: {entry.Key}" +
                    $"\n  → Avg: {stat.AverageMs:F2}ms (± {stat.StandardDeviationMs:F2}ms)" +
                    $"\n  → Range: {stat.MinMs}ms - {stat.MaxMs}ms (95%: {stat.Percentile95Ms:F2}ms)" +
                    $"\n  → Throughput: {stat.RequestsPerSecond:F2} req/sec" +
                    $"\n  → Relative: {(stat.RelativePerformanceRatio * 100):F2}% of fastest" +
                    $"\n  → Consistency: {(stat.VariabilityCoefficient < 0.1 ? "High" : stat.VariabilityCoefficient < 0.3 ? "Medium" : "Low")}" +
                    $"\n  → Data Payload: ~{stat.EstimatedPropertyCount} properties across ~{stat.EstimatedEntityCount} entities"
                );
            }
            if (sortedStats.Count > 1)
            {
                _logger.LogInformation("=== COMPARATIVE ANALYSIS ===");
                var fastest = sortedStats.First();
                var slowest = sortedStats.Last();
                var speedupFactor = Math.Round(slowest.Value.AverageMs / fastest.Value.AverageMs, 2);
                _logger.LogInformation(
                    $"• {fastest.Key} is {speedupFactor}x faster than {slowest.Key}" +
                    $"\n• Potential time savings: {slowest.Value.AverageMs - fastest.Value.AverageMs:F2}ms per request" +
                    $"\n• At 1000 requests, this would save {(slowest.Value.AverageMs - fastest.Value.AverageMs) * 1000 / 1000:F2} seconds"
                );
                var complexityAnalysis = GetComplexityAnalysis();
                _logger.LogInformation("=== QUERY COMPLEXITY ANALYSIS ===");
                foreach (var spec in sortedStats)
                {
                    var metrics = complexityAnalysis.SpecificationMetrics[spec.Key];
                    _logger.LogInformation(
                        $"• {spec.Key}:" +
                        $"\n  → Includes: {metrics.IncludeCount}" +
                        $"\n  → Est. Joins: {metrics.EstimatedJoinCount}" +
                        $"\n  → Projection Depth: {metrics.ProjectionDepth}" +
                        $"\n  → Relationship Depth: {metrics.RelationshipDepth}" +
                        $"\n  → Calculated Properties: {metrics.CalculatedPropertyCount}" +
                        $"\n  → Performance/Complexity Ratio: {Math.Round(spec.Value.AverageMs / metrics.EstimatedJoinCount, 2)}ms per join"
                    );
                }
                var mostConsistent = sortedStats.OrderBy(s => s.Value.VariabilityCoefficient).First();
                _logger.LogInformation(
                    $"• Most consistent: {mostConsistent.Key} (CV: {mostConsistent.Value.VariabilityCoefficient:F2})"
                );
                var recommendation = sortedStats
                    .OrderBy(s => s.Value.Rank + s.Value.VariabilityCoefficient * 5)
                    .First();
                _logger.LogInformation(
                    $"• Recommendation: {recommendation.Key} provides the best balance of speed and consistency"
                );
                _logger.LogInformation("=== EFFICIENCY ANALYSIS ===");
                foreach (var spec in sortedStats)
                {
                    double msPerProperty = spec.Value.AverageMs / spec.Value.EstimatedPropertyCount;
                    _logger.LogInformation(
                        $"• {spec.Key}: {msPerProperty:F2}ms per property fetched"
                    );
                }
                var bestEfficiency = sortedStats
                    .OrderBy(s => s.Value.AverageMs / s.Value.EstimatedPropertyCount)
                    .First();
                _logger.LogInformation(
                    $"• Most data-efficient: {bestEfficiency.Key}"
                );
            }
        }
    }
}
