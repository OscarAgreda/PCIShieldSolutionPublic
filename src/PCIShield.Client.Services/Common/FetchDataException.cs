namespace PCIShield.Client.Services.Common
{
    public class FetchDataException : Exception
    {
        public FetchDataException()
        { }
        public FetchDataException(string message) : base(message)
        {
        }
        public FetchDataException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}