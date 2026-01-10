using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShieldLib.SharedKernel.Interfaces;
namespace BlazorMauiShared.Models.ApplicationUser
{
    public class SearchApplicationUserRequest
    {
        public string SearchTerm { get; set; }
    }
    public class ListApplicationUserRequest : BaseRequest
    {
    }
    public class GetApplicationUserListRequest
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
    public class FilteredApplicationUserRequest : GetApplicationUserListRequest
    {
        public FilteredApplicationUserRequest()
        {
            Filters = new Dictionary<string, string>();
            Sorting = new List<Sort>();
        }
        
        public Dictionary<string, string> Filters { get; set; }
        public List<Sort> Sorting { get; set; }
    }
}

