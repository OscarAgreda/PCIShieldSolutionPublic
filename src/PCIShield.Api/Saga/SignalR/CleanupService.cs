using System;
using System.Threading;
namespace PCIShield.Api.Saga.SignalR
{
    public class CleanupService
    {
        private readonly ProcessedMessagesStore _messagesStore;
        private Timer _cleanupTimer;
        public CleanupService(ProcessedMessagesStore messagesStore)
        {
            this._messagesStore = messagesStore;
        }
        public void Start()
        {
            this._cleanupTimer = new Timer(this.Cleanup, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        }
        public void Stop()
        {
            this._cleanupTimer?.Dispose();
        }
        private void Cleanup(object state)
        {
            this._messagesStore.CleanupOldMessages(TimeSpan.FromHours(2));
        }
    }
}