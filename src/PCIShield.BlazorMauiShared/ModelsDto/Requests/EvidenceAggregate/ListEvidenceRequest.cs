using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShieldLib.SharedKernel.Interfaces;
namespace BlazorMauiShared.Models.Evidence
{
    public class SearchEvidenceRequest
    {
        public string SearchTerm { get; set; }
    }
    public class ListEvidenceRequest : BaseRequest
    {
    }
    public class GetEvidenceListRequest
    {
        private int _pageSize = 10;
        private int _pageNumber = 1;
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? 10 : value;
        }
        
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value <= 0 ? 1 : value;
        }
        public Guid MerchantId { get; set; }
        public Guid? AssetId { get; set; }
        public Guid? PaymentChannelId { get; set; }
        public Guid? PaymentPageId { get; set; }
    }
    public class FilteredEvidenceRequest : GetEvidenceListRequest
    {
        public FilteredEvidenceRequest()
        {
            Filters = new Dictionary<string, string>();
            Sorting = new List<Sort>();
        }
        
        public Dictionary<string, string> Filters { get; set; }
        public List<Sort> Sorting { get; set; }
    }
}

