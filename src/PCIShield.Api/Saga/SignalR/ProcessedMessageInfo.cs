using System;
namespace PCIShield.Api.Saga.SignalR
{
    public class ProcessedMessageInfo
    {
        public Guid MessageId { get; set; }
        public DateTime ProcessedTime { get; set; }
    }
}