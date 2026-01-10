using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
namespace PCIShieldLib.SharedKernel.Interfaces
{
    public interface IFilter
    {
        string Field { get; }
        string Operator { get; }
        string Value { get; }
    }
    public interface ISort
    {
        string Field { get; }
        SortDirection Direction { get; }
    }
    public enum SortDirection
    {
        Ascending,
        Descending
    }
    public enum FilterOperator
    {
        Equals,
        NotEquals,
        Contains,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Between,
        In,
        NotIn,
        StartsWith,
        EndsWith,
        NotContains,
        Equal,
        NotEqual,
        Empty,
        NotEmpty
    }
    public class Filter : IFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    public class Sort : ISort
    {
        public string Field { get; set; }
        public SortDirection Direction { get; set; }
    }
    public static class GridStateConverter
    {
        private static string ConvertFilterOperator(FilterOperator op)
        {
            return op switch
            {
                FilterOperator.Contains => "contains",
                FilterOperator.NotContains => "notcontains",
                FilterOperator.Equal => "eq",
                FilterOperator.NotEqual => "ne",
                FilterOperator.GreaterThan => "gt",
                FilterOperator.GreaterThanOrEqual => "gte",
                FilterOperator.LessThan => "lt",
                FilterOperator.LessThanOrEqual => "lte",
                FilterOperator.StartsWith => "startswith",
                FilterOperator.EndsWith => "endswith",
                FilterOperator.Empty => "empty",
                FilterOperator.NotEmpty => "notempty",
                _ => "contains"
            };
        }
    }
    public class AuditBase
    {
        public string AuditId { get; set; }
        public string AuditDataBeforeChanged { get; set; }
        public string AuditDataAfterChanged { get; set; }
        public string AuditEntityId { get; set; }
        public DateTime AuditTimeStamp { get; set; }
        public string PerformedByUserId { get; set; }
        public string AuditIpAddress { get; set; }
        [NotMapped]
        public string AuditUserDeviceInfo { get; set; }
        [NotMapped]
        public string AuditFrontEndUserAppInfo { get; set; }
        public string AuditBackEndVersion { get; set; }
        public string AuditBoundedContextId { get; set; }
        public string AuditAction { get; set; }
        public string AuditEntityType { get; set; }
        public string AuditEntityName { get; set; }
    }
}