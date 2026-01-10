using System;
using Ardalis.Specification;
using PCIShield.Domain.ModelsDto;
namespace PCIShield.BlazorMauiShared.Models
{
    public abstract class BaseResponse : BaseMessage
    {
        public BaseResponse(Guid correlationId)
        {
            _correlationId = correlationId;
        }
        public BaseResponse()
        {
        }
        public bool IsSuccess { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string OperationElapsedTime { get; set; } = String.Empty;
        public string? OperationTypeNameId { get; set; } = String.Empty;
    }
    public class SpecificationPerformanceResponse<TDto> : BaseResponse where TDto : class
    {
        public SpecificationPerformanceResponse(Guid correlationId) : base(correlationId) { }
        public SpecificationPerformanceResponse() { }
        public Dictionary<string, List<long>> TimingResults { get; set; } = new();
        public Dictionary<string, PerformanceStats> Statistics { get; set; } = new();
        public QueryComplexityAnalysis ComplexityAnalysis { get; set; }
        public string ErrorMessage { get; set; }
        public string FastestSpecification { get; set; }
        public string MostConsistentSpecification { get; set; }
        public string RecommendedSpecification { get; set; }
        public double MaximumSpeedupFactor { get; set; }
        public class PerformanceStats
        {
            public double AverageMs { get; set; }
            public long MinMs { get; set; }
            public long MaxMs { get; set; }
            public int RunCount { get; set; }
            public double StandardDeviationMs { get; set; }
            public double MedianMs { get; set; }
            public double Percentile95Ms { get; set; }
            public double RelativePerformanceRatio { get; set; }
            public int Rank { get; set; }
            public double RequestsPerSecond { get; set; }
            public double VariabilityCoefficient { get; set; }
            public int EstimatedPropertyCount { get; set; }
            public int EstimatedEntityCount { get; set; }
            public string PerformanceCategory { get; set; }
        }
        public class QueryComplexityAnalysis
        {
            public Dictionary<string, SpecComplexityMetrics> SpecificationMetrics { get; set; } = new();
            public class SpecComplexityMetrics
            {
                public int EstimatedJoinCount { get; set; }
                public int IncludeCount { get; set; }
                public int ProjectionDepth { get; set; }
                public bool UsesSplitQuery { get; set; }
                public bool UsesNoTracking { get; set; }
                public int RelationshipDepth { get; set; }
                public string ComplexityLevel { get; set; }
                public int CalculatedPropertyCount { get; set; }
            }
        }
        public void SetSummaryInfo()
        {
            if (Statistics == null || Statistics.Count == 0)
                return;
            var ordered = Statistics.OrderBy(s => s.Value.AverageMs).ToList();
            FastestSpecification = ordered.First().Key;
            if (ordered.Count > 1)
            {
                var mostConsistent = Statistics.OrderBy(s => s.Value.VariabilityCoefficient).First().Key;
                MostConsistentSpecification = mostConsistent;
                var recommended = Statistics
                    .OrderBy(s => s.Value.Rank + s.Value.VariabilityCoefficient * 5)
                    .First().Key;
                RecommendedSpecification = recommended;
                var fastest = ordered.First().Value;
                var slowest = ordered.Last().Value;
                MaximumSpeedupFactor = slowest.AverageMs / fastest.AverageMs;
            }
        }
    }
    }