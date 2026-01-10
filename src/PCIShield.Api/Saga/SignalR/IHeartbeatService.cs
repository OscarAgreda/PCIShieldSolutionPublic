namespace PCIShield.Api.Saga.SignalR
{
    public interface IHeartbeatService
    {
        void Start();
        void Stop();
    }
}