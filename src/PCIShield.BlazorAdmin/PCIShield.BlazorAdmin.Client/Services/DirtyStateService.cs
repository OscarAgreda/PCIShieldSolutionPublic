using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
namespace PCIShield.BlazorAdmin.Client.Services
{
    public interface IErrorReportingService
    {
        Task ReportErrorAsync(ErrorReport error);
    }
    public class ErrorReport
    {
        public Exception Exception { get; set; }
        public string Context { get; set; }
        public DateTime Timestamp { get; set; }
        public string ComponentName { get; set; }
        public Guid CustomerId { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> AdditionalData { get; set; }
    }
    public interface IDirtyStateService : IDisposable
    {
        bool IsDirty { get; }
        void SetDirty(bool isDirty);
        IDisposable TrackChanges();
        event EventHandler<bool> DirtyStateChanged;
    }
    public class ResilientDirtyStateService : IDirtyStateService
    {
        private readonly ILogger<ResilientDirtyStateService> _logger;
        private readonly Subject<bool> _dirtyStateSubject = new();
        private readonly CompositeDisposable _disposables = new();
        private bool _isDirty;
        private bool _isDisposed;
        private readonly Queue<bool> _pendingStateChanges = new();
        public bool IsDirty => _isDirty;
        public event EventHandler<bool> DirtyStateChanged;
        public ResilientDirtyStateService(
            ILogger<ResilientDirtyStateService> logger)
        {
            _logger = logger;
            _disposables.Add(
                _dirtyStateSubject
                    .Throttle(TimeSpan.FromMilliseconds(300))
                    .Subscribe(
                        onNext: ProcessStateChange,
                        onError: ex => _logger.LogError(ex, "Error in dirty state stream")
                    )
            );
        }
        public void SetDirty(bool isDirty)
        {
            if (_isDisposed) return;
            try
            {
                    _dirtyStateSubject.OnNext(isDirty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting dirty state");
            }
        }
        private void ProcessStateChange(bool isDirty)
        {
            try
            {
                if (_isDisposed) return;
                _isDirty = isDirty;
                    DirtyStateChanged?.Invoke(this, isDirty);
            }
            catch (JSDisconnectedException)
            {
                _logger.LogInformation("JS disconnected while processing state change");
                _pendingStateChanges.Enqueue(isDirty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing state change");
            }
        }
        private void ProcessPendingChanges()
        {
            while (_pendingStateChanges.Count > 0)
            {
                var state = _pendingStateChanges.Dequeue();
                ProcessStateChange(state);
            }
        }
        public IDisposable TrackChanges()
        {
            if (_isDisposed)
                return Disposable.Empty;
            SetDirty(true);
            return new DisposableAction(() =>
            {
                try
                {
                    if (!_isDisposed)
                        SetDirty(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in TrackChanges disposal");
                }
            });
        }
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            try
            {
                _dirtyStateSubject?.OnCompleted();
                _disposables?.Dispose();
                _dirtyStateSubject?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing DirtyStateService");
            }
        }
        private class DisposableAction : IDisposable
        {
            private readonly Action _action;
            private bool _isDisposed;
            public DisposableAction(Action action)
            {
                _action = action;
            }
            public void Dispose()
            {
                if (_isDisposed) return;
                _isDisposed = true;
                _action?.Invoke();
            }
        }
    }
}