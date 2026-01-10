// PCIShield – Override infrastructure (LanguageExt-free edition)

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PCIShield.Client.Services.Merchant;
using PCIShield.Domain.ModelsDto;

using Unit = System.Reactive.Unit;

namespace PCIShield.BlazorAdmin.Client.Infrastructure
{
    public sealed class AppError
    {
        public string Message { get; }
        public Exception? Cause { get; }

        public AppError(string message, Exception? cause = null)
        {
            Message = message;
            Cause = cause;
        }

        public override string ToString() => Message;
    }

    public readonly struct Result
    {
        public bool IsSuccess { get; }
        public AppError? Error { get; }

        private Result(bool success, AppError? error)
        {
            IsSuccess = success;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Fail(string message) => new(false, new AppError(message));
        public static Result Fail(AppError error) => new(false, error);
        public static implicit operator bool(Result r) => r.IsSuccess;
    }
    public interface IOverridable<TValue>
    {
        bool IsOverridden { get; }
        TValue Value { get; }
    }

    public interface IOverridableWriter<TValue>
    {
        void SetOverridden(bool value);
        void SetValue(TValue value);
    }

    public abstract class OverridableFieldBase<TEntity, TValue> :
        IOverridable<TValue>, IOverridableWriter<TValue>
    {
        protected TEntity Entity { get; }

        protected OverridableFieldBase(TEntity entity) =>
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));

        public abstract bool IsOverridden { get; }
        public abstract TValue Value { get; }

        public abstract void SetOverridden(bool value);
        public abstract void SetValue(TValue value);
    }
    public sealed class OverrideManager<TSlot, TValue>
        where TSlot : class, IOverridable<TValue>, IOverridableWriter<TValue>
    {
        private readonly TSlot _slot;
        private readonly Action _notify;

        public OverrideManager(TSlot slot, Action notifyStateChanged)
        {
            _slot = slot ?? throw new ArgumentNullException(nameof(slot));
            _notify = notifyStateChanged ??
                       throw new ArgumentNullException(nameof(notifyStateChanged));
        }

        public void Toggle(bool isOverridden)
        {
            _slot.SetOverridden(isOverridden);
            _notify();
        }

        public void Reset(TValue defaultValue)
        {
            _slot.SetValue(defaultValue);
            _slot.SetOverridden(false);
            _notify();
        }
    }
    public sealed class OperationResult
    {
        public string OperationName { get; init; } = string.Empty;
        public bool Success { get; init; }
        public AppError? Error { get; init; }
        public Dictionary<string, object> Context { get; } = new();

        public static OperationResult Succeeded(string name) =>
            new() { OperationName = name, Success = true };

        public static OperationResult Failed(string name, AppError err) =>
            new() { OperationName = name, Success = false, Error = err };

        public OperationResult WithContext(string key, object value)
        {
            Context[key] = value;
            return this;
        }
    }

    public sealed class TransactionExecutionSummary
    {
        public IReadOnlyList<OperationResult> Steps { get; }
        public bool Success => Steps.All(s => s.Success);

        public TransactionExecutionSummary(IEnumerable<OperationResult> steps) =>
            Steps = steps.ToList();
    }

    public sealed class TransactionBuilder
    {
        private readonly List<(string Name, Func<CancellationToken, Task<Result>> Op)> _steps = new();
        private readonly ILogger _logger;

        public TransactionBuilder(ILogger logger) => _logger = logger;

        public TransactionBuilder Step(string name, Func<CancellationToken, Task<Result>> op)
        {
            _steps.Add((name, op));
            return this;
        }

        public async Task<TransactionExecutionSummary> ExecuteAsync(
            CancellationToken cancellationToken = default,
            bool continueOnError = true)
        {
            var results = new List<OperationResult>();

            foreach (var (name, op) in _steps)
            {
                _logger.LogDebug("Executing operation '{Name}'", name);

                if (cancellationToken.IsCancellationRequested)
                {
                    results.Add(OperationResult.Failed(name, new AppError("Operation cancelled")));
                    break;
                }

                try
                {
                    var res = await op(cancellationToken);

                    if (res)
                        results.Add(OperationResult.Succeeded(name));
                    else
                    {
                        _logger.LogWarning("Operation '{Name}' failed: {Error}", name, res.Error);
                        results.Add(OperationResult.Failed(name, res.Error!));
                        if (!continueOnError) break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in operation '{Name}'", name);
                    results.Add(OperationResult.Failed(name, new AppError(ex.Message, ex)));
                    if (!continueOnError) break;
                }
            }

            return new TransactionExecutionSummary(results);
        }
    }
    public abstract class OverriddenFieldPersister<TSlot, TValue>
        where TSlot : class, IOverridable<TValue>
    {
        protected ILogger Logger { get; }

        protected OverriddenFieldPersister(ILogger logger) => Logger = logger;

        protected virtual bool AreEqual(TValue a, TValue b) => Equals(a, b);

        protected virtual bool IsEmptyValue(TValue value) =>
            value == null || (value is string s && string.IsNullOrWhiteSpace(s));

        public async Task<Result> ApplyAsync(
            TSlot slot,
            IHttpMerchantClientService api,
            CancellationToken token = default)
        {
            try
            {
                if (!slot.IsOverridden || IsEmptyValue(slot.Value))
                {
                    Logger.LogDebug("Field not overridden or empty, skipping persistence.");
                    return Result.Success();
                }

                Logger.LogDebug("Persisting overridden field…");

                bool exists = await ExistsAsync(slot, api, token);

                if (exists && AreEqual(slot.Value, GetPersistentValue(slot)))
                    return Result.Success();

                return exists
                    ? await UpdateAsync(slot, api, token)
                    : await CreateAsync(slot, api, token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error persisting field");
                return Result.Fail(new AppError(ex.Message, ex));
            }
        }

        protected abstract TValue GetPersistentValue(TSlot slot);
        protected abstract ValueTask<bool> ExistsAsync(TSlot slot,
                                                       IHttpMerchantClientService api,
                                                       CancellationToken token);
        protected abstract Task<Result> UpdateAsync(TSlot slot,
                                                    IHttpMerchantClientService api,
                                                    CancellationToken token);
        protected abstract Task<Result> CreateAsync(TSlot slot,
                                                    IHttpMerchantClientService api,
                                                    CancellationToken token);
    }
}
