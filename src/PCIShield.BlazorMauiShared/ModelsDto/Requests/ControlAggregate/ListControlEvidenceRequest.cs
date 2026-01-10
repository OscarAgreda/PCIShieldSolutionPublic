using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShieldLib.SharedKernel.Interfaces;
namespace BlazorMauiShared.Models.ControlEvidence
{
    public class SearchControlEvidenceRequest
    {
        public string SearchTerm { get; set; }
    }
    public class ListControlEvidenceRequest : BaseRequest
    {
    }
    public class GetControlEvidenceListRequest
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
        public Guid AssessmentId { get; set; }
        public Guid ControlId { get; set; }
        public Guid EvidenceId { get; set; }
    }
    public class FilteredControlEvidenceRequest : GetControlEvidenceListRequest
    {
        public FilteredControlEvidenceRequest()
        {
            Filters = new Dictionary<string, string>();
            Sorting = new List<Sort>();
        }
        
        public Dictionary<string, string> Filters { get; set; }
        public List<Sort> Sorting { get; set; }
    }
}

