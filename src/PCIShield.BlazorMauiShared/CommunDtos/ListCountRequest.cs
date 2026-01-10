using System;

using PCIShield.BlazorMauiShared.Models;
namespace PCIShield.BlazorMauiShared.ModelsDto.Requests.DashboardAggregate
{
    public class SearchCountDataLinkRequest
    {
        public string SearchTerm { get; set; }
    }
    public class ListCountDataLinkRequest : BaseRequest
    {
    }
    public class GetCountDataLinkRequest
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
    }
    public class FilteredCountDataLinkRequest : ListCountDataLinkRequest
    {
        public FilteredCountDataLinkRequest()
        {
            Filters = new Dictionary<string, string>();
            Sorting = new List<Sort>();
        }
        public Dictionary<string, string> Filters { get; set; }
        public List<Sort> Sorting { get; set; }
    }
}
namespace PCIShield.BlazorMauiShared.ModelsDto.Responses.DashboardAggregate
{
    public class CountDataLinkResponse : BaseResponse
    {
        public CountDataLinkResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        public CountDataLinkResponse()
        {
        }
        public int TotalCountInvoice { get; set; }
        public int TotalCountProduct { get; set; }
        public int TotalCountSuppliers { get; set; }
        public int TotalCountCustomer { get; set; }

    }
}
