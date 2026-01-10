using System;
using System.Collections.Generic;
using System.Linq;
namespace PCIShield.Api.Saga.SignalR
{
    public class ProcessedMessagesStore
    {
        private readonly Dictionary<Guid, ProcessedMessageInfo> _processedMessages = new Dictionary<Guid, ProcessedMessageInfo>();
        public void CleanupOldMessages(TimeSpan maxAge)
        {
            var cutoff = DateTime.UtcNow - maxAge;
            var oldKeys = this._processedMessages.Values
                .Where(info => info.ProcessedTime < cutoff)
                .Select(info => info.MessageId)
                .ToList();
            foreach (var key in oldKeys)
            {
                this._processedMessages.Remove(key);
            }
        }
        public bool IsProcessed(Guid messageId)
        {
            return this._processedMessages.ContainsKey(messageId);
        }
        public void MarkAsProcessed(Guid messageId)
        {
            this._processedMessages[messageId] = new ProcessedMessageInfo
            {
                MessageId = messageId,
                ProcessedTime = DateTime.UtcNow
            };
        }
    }
}