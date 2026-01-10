namespace PCIShield.BlazorAdmin.Services
{
    public interface IDashBoardStateService
    {
        int TotalInvoiceCount { get; set; }
        int TotalProductCount { get; set; }
        int TotalSupplierCount { get; set; }
        int TotalMerchantCount { get; set; }

        bool IsDataLoaded { get; set; }

        event Action OnChange;
        void NotifyStateChanged();
    }

    public class DashBoardStateService : IDashBoardStateService
    {

        private int _totalInvoiceCount;
        private int _totalProductCount;
        private int _totalSupplierCount;
        private int _totalMerchantCount;
        private bool _isDataLoaded;
        public int TotalInvoiceCount
        {
            get => _totalInvoiceCount;
            set
            {
                _totalInvoiceCount = value;
                NotifyStateChanged();
            }
        }

        public int TotalProductCount
        {
            get => _totalProductCount;
            set
            {
                _totalProductCount = value;
                NotifyStateChanged();
            }
        }

        public int TotalSupplierCount
        {
            get => _totalSupplierCount;
            set
            {
                _totalSupplierCount = value;
                NotifyStateChanged();
            }
        }

        public int TotalMerchantCount
        {
            get => _totalMerchantCount;
            set
            {
                _totalMerchantCount = value;
                NotifyStateChanged();
            }
        }

        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set
            {
                _isDataLoaded = value;
                NotifyStateChanged();
            }
        }
        public event Action OnChange;

        public void NotifyStateChanged()
        {
            try
            {
                if (OnChange != null)
                {
                    OnChange.Invoke();
                }
            }
            catch (System.Exception ex)
            {
            	
            }
         
        }
    }
}
