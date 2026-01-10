namespace PCIShield.Client.Services.Common
{
    public interface IUserMessageService
    {
        event Action<string> MessageChanged;
        void ShowMessage(string message);
    }
    public class UserMessageService : IUserMessageService
    {
        public event Action<string> MessageChanged;
        public void ShowMessage(string message)
        {
            MessageChanged?.Invoke(message);
        }
    }
}