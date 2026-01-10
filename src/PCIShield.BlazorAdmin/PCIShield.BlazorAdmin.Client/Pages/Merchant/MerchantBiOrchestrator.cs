using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using LanguageExt;

using PCIShield.Domain.ModelsDto;

using static LanguageExt.Prelude;
using static PCIShield.BlazorAdmin.Client.Pages.Merchant.ComplianceForecaster;

using static PCIShield.BlazorAdmin.Client.Pages.Merchant.MerchantBiDashboardPage;

namespace PCIShield.BlazorAdmin.Client.Pages.Merchant
{
    public enum RiskLevel { Low, Medium, High, Critical }
    public enum TrendDirection { Up, Down, Stable }
    public enum MaturityLevel { Initial, Managed, Defined, Quantified, Optimizing }

    public record RiskIndicator(string Name, decimal Value, string Description);
    public record RiskFactor(string Name, decimal Weight, decimal Score);
    public record Percentile(string Metric, decimal Value, int Rank);
    public record ImprovementOpportunity(string Area, decimal PotentialGain, string Action);
    public record SignificantCorrelation(string Factor1, string Factor2, decimal Coefficient);
    public record CausalityHypothesis(string Theory, decimal Confidence);
    public record SeasonalPattern(string Pattern, decimal Strength, int Period);
    public record KPIThresholds(Dictionary<string, decimal> Thresholds);
    public record KPIAlert(Guid MerchantId, string KPI, decimal CurrentValue, decimal Threshold, string Severity, TrendDirection TrendDirection);
    public record AnomalyAlert(Guid MerchantId, string Type, string Severity, DateTime DetectedAt, string Recommendation);
    public record ForecastUpdate(DateTime Timestamp, ForecastResult Result);
    public record ComplianceFailureScenario(string Type, decimal Severity);
    public record MitigationStrategy(string Strategy, decimal Effectiveness);
    public record AutomatedResponse(string Action, decimal Confidence);
    public record ImpactAssessment(decimal FinancialImpact, decimal OperationalImpact);
    public record Alert(Guid Id, string Type, string Message, DateTime Timestamp);
    public record CapabilityScore(string Capability, decimal Score);
    public record MaturityGap(string Area, decimal CurrentScore, decimal TargetScore);
    public record RoadmapToNextLevel(List<string> Steps, TimeSpan EstimatedTime);

    public class MerchantRiskAnalyzer
    {
        private readonly BehaviorSubject<RiskProfile> _riskStream = new(new RiskProfile(0, new(), RiskLevel.Low, None, new List<RiskIndicator>()));

        public record RiskProfile(
            decimal OverallScore,
            Dictionary<string, decimal> RiskFactors,
            RiskLevel Level,
            Option<DateTime> NextReviewDate,
            IEnumerable<RiskIndicator> Indicators);

        public Either<string, RiskProfile> CalculateMerchantRisk(MerchantDto merchant)
        {
            try
            {
                var vulnScore = CalculateVulnerabilityRisk(merchant);
                var complianceScore = CalculateComplianceRisk(merchant);
                var volumeRisk = CalculateVolumeRisk(merchant.AnnualCardVolume, merchant.MerchantLevel);
                var assessmentRisk = CalculateAssessmentRisk(merchant);

                var weightedScore = ComputeWeightedRiskScore(
                    (vulnScore, 0.35m),
                    (complianceScore, 0.30m),
                    (volumeRisk, 0.20m),
                    (assessmentRisk, 0.15m));

                return new RiskProfile(
                    OverallScore: weightedScore,
                    RiskFactors: new Dictionary<string, decimal>
                    {
                        ["Vulnerability"] = vulnScore,
                        ["Compliance"] = complianceScore,
                        ["Volume"] = volumeRisk,
                        ["Assessment"] = assessmentRisk
                    },
                    Level: DetermineRiskLevel(weightedScore),
                    NextReviewDate: CalculateNextReviewDate(weightedScore),
                    Indicators: ExtractRiskIndicators(merchant, weightedScore));
            }
            catch (Exception ex)
            {
                return Left($"Risk calculation failed: {ex.Message}");
            }
        }

        private decimal CalculateVulnerabilityRisk(MerchantDto merchant) =>
            merchant.ComplianceRank switch
            {
                >= 90 => 10m,
                >= 75 => 30m,
                >= 50 => 60m,
                _ => 85m
            };

        private decimal CalculateComplianceRisk(MerchantDto merchant) =>
            100m - merchant.ComplianceRank;

        private decimal CalculateVolumeRisk(decimal volume, int level) =>
            level switch
            {
                1 => volume > 6000000 ? 80m : 60m,
                2 => volume > 1000000 ? 60m : 40m,
                3 => volume > 20000 ? 40m : 20m,
                _ => 20m
            };

        private decimal CalculateAssessmentRisk(MerchantDto merchant)
        {
            var daysSinceAssessment = (DateTime.Now - merchant.LastAssessmentDate).Value.Days;
            return daysSinceAssessment switch
            {
                > 365 => 90m,
                > 180 => 60m,
                > 90 => 30m,
                _ => 10m
            };
        }

        private decimal ComputeWeightedRiskScore(params (decimal score, decimal weight)[] factors) =>
            factors.Sum(f => f.score * f.weight);

        private RiskLevel DetermineRiskLevel(decimal score) => score switch
        {
            >= 75 => RiskLevel.Critical,
            >= 50 => RiskLevel.High,
            >= 25 => RiskLevel.Medium,
            _ => RiskLevel.Low
        };

        private Option<DateTime> CalculateNextReviewDate(decimal riskScore) =>
            riskScore > 50
                ? Some(DateTime.Now.AddDays(7))
                : Some(DateTime.Now.AddDays(30));

        private IEnumerable<RiskIndicator> ExtractRiskIndicators(MerchantDto merchant, decimal score)
        {
            yield return new RiskIndicator("Overall Risk", score, $"Risk level: {DetermineRiskLevel(score)}");
            yield return new RiskIndicator("Compliance", merchant.ComplianceRank, "Compliance score");
            yield return new RiskIndicator("Volume", (decimal)merchant.AnnualCardVolume, "Annual card volume");
        }

        public IObservable<AnomalyAlert> DetectAnomalies(IObservable<MerchantDto> merchantStream) =>
            merchantStream
                .Buffer(TimeSpan.FromMinutes(5))
                .Where(batch => batch.Any())
                .SelectMany(batch =>
                {
                    var avgCompliance = batch.Average(m => m.ComplianceRank);
                    var stdDev = CalculateStandardDeviation(batch.Select(m => (decimal)m.ComplianceRank));

                    return batch
                        .Where(m => Math.Abs(m.ComplianceRank - (int)avgCompliance) > stdDev * 2)
                        .Select(m => new AnomalyAlert(
                            MerchantId: m.MerchantId,
                            Type: "Compliance Deviation",
                            Severity: "High",
                            DetectedAt: DateTime.UtcNow,
                            Recommendation: "Review merchant compliance immediately"));
                });

        private decimal CalculateStandardDeviation(IEnumerable<decimal> values)
        {
            var avg = values.Average();
            var sumSquaredDiff = values.Sum(v => Math.Pow((double)(v - avg), 2));
            return (decimal)Math.Sqrt(sumSquaredDiff / values.Count());
        }
    }

    public class CompliancePredictionEngine
    {
        public record CompliancePrediction(
            decimal ProbabilityOfNonCompliance,
            DateTime PredictionHorizon,
            IEnumerable<RiskFactor> ContributingFactors,
            Option<string> RecommendedAction);

        public Either<string, CompliancePrediction> PredictComplianceStatus(
            MerchantDto merchant,
            IEnumerable<AssessmentDto> historicalAssessments)
        {
            try
            {
                var factors = new List<RiskFactor>
                {
                    new("Historical Compliance", 0.4m, CalculateHistoricalScore(historicalAssessments)),
                    new("Current Status", 0.3m, merchant.ComplianceRank),
                    new("Assessment Frequency", 0.3m, CalculateAssessmentFrequencyScore(historicalAssessments))
                };

                var probability = factors.Sum(f => f.Weight * f.Score) / 100m;

                return new CompliancePrediction(
                    ProbabilityOfNonCompliance: probability,
                    PredictionHorizon: DateTime.Now.AddMonths(3),
                    ContributingFactors: factors,
                    RecommendedAction: probability > 0.7m
                        ? Some("Immediate assessment required")
                        : None);
            }
            catch (Exception ex)
            {
                return Left($"Prediction failed: {ex.Message}");
            }
        }

        private decimal CalculateHistoricalScore(IEnumerable<AssessmentDto> assessments) =>
            assessments.Any()
                ? 100m - (decimal)assessments.Where(a => a.ComplianceScore != null)
                    .Average(a => a.ComplianceScore!)
                : 50m;

        private decimal CalculateAssessmentFrequencyScore(IEnumerable<AssessmentDto> assessments)
        {
            if (!assessments.Any())
                return 100m;
            var avgDaysBetween = assessments
                .OrderBy(a => a.StartDate)
                .Zip(assessments.Skip(1), (a, b) => (b.StartDate - a.StartDate).Days)
                .DefaultIfEmpty(365)
                .Average();
            return Math.Min(100m, (decimal)avgDaysBetween / 3.65m);
        }
    }

    public class MerchantBenchmarkAnalyzer
    {
        public record BenchmarkResult(
            decimal PerformanceIndex,
            int PeerRanking,
            Dictionary<string, Percentile> MetricPercentiles,
            IEnumerable<ImprovementOpportunity> Opportunities);

        public Either<string, BenchmarkResult> BenchmarkAgainstPeers(
            MerchantDto merchant,
            IEnumerable<MerchantDto> peerGroup)
        {
            try
            {
                var validPeers = peerGroup.Where(p => p.MerchantLevel == merchant.MerchantLevel).ToList();
                if (!validPeers.Any())
                    return Left("No valid peers found");

                var percentiles = new Dictionary<string, Percentile>
                {
                    ["Compliance"] = CalculatePercentile("Compliance", merchant.ComplianceRank,
                        validPeers.Select(p => (decimal)p.ComplianceRank)),
                    ["Volume"] = CalculatePercentile("Volume", merchant.AnnualCardVolume,
                        validPeers.Select(p => p.AnnualCardVolume))
                };

                var performanceIndex = percentiles.Values.Average(p => p.Value);
                var ranking = validPeers.Count(p => p.ComplianceRank > merchant.ComplianceRank) + 1;

                var opportunities = GenerateOpportunities(percentiles);

                return new BenchmarkResult(
                    PerformanceIndex: performanceIndex,
                    PeerRanking: ranking,
                    MetricPercentiles: percentiles,
                    Opportunities: opportunities);
            }
            catch (Exception ex)
            {
                return Left($"Benchmarking failed: {ex.Message}");
            }
        }

        private Percentile CalculatePercentile(string metric, decimal value, IEnumerable<decimal> population)
        {
            var sorted = population.OrderBy(v => v).ToList();
            var rank = sorted.Count(v => v <= value);
            var percentile = (decimal)rank / sorted.Count * 100;
            return new Percentile(metric, percentile, rank);
        }

        private IEnumerable<ImprovementOpportunity> GenerateOpportunities(Dictionary<string, Percentile> percentiles)
        {
            foreach (var kvp in percentiles.Where(p => p.Value.Value < 50))
            {
                yield return new ImprovementOpportunity(
                    kvp.Key,
                    50m - kvp.Value.Value,
                    $"Improve {kvp.Key} to reach median performance");
            }
        }
    }

    public class ComplianceForecaster
    {
        private readonly Subject<ForecastUpdate> _forecastStream = new();

        public record ForecastResult(
            Dictionary<DateTime, decimal> Predictions,
            decimal ConfidenceInterval,
            TrendDirection Trend,
            IEnumerable<SeasonalPattern> Patterns);

        public IObservable<ForecastResult> ForecastCompliance(
            IEnumerable<TimeSeriesPoint> historicalData,
            int horizonMonths) =>
            Observable.Start(() =>
            {
                var predictions = new Dictionary<DateTime, decimal>();
                var trend = AnalyzeTrend(historicalData);
                var lastValue = historicalData.LastOrDefault()?.Value ?? 85m;

                for (int i = 1; i <= horizonMonths; i++)
                {
                    var trendAdjustment = trend == TrendDirection.Up ? 2m :
                                         trend == TrendDirection.Down ? -2m : 0m;
                    var predictedValue = Math.Max(0, Math.Min(100, lastValue + (trendAdjustment * i)));
                    predictions[DateTime.Now.AddMonths(i)] = predictedValue;
                }

                return new ForecastResult(
                    Predictions: predictions,
                    ConfidenceInterval: 85m,
                    Trend: trend,
                    Patterns: new List<SeasonalPattern>());
            })
            .Do(result => _forecastStream.OnNext(new ForecastUpdate(DateTime.Now, result)));

        private TrendDirection AnalyzeTrend(IEnumerable<TimeSeriesPoint> data)
        {
            if (!data.Any()) return TrendDirection.Stable;
            var values = data.Select(d => d.Value).ToList();
            var firstHalf = values.Take(values.Count / 2).Average();
            var secondHalf = values.Skip(values.Count / 2).Average();

            return secondHalf > firstHalf + 5 ? TrendDirection.Up :
                   secondHalf < firstHalf - 5 ? TrendDirection.Down :
                   TrendDirection.Stable;
        }
    }

    public class AlertPrioritizationEngine
    {
        public record PrioritizedAlert(
            Alert BaseAlert,
            decimal Priority,
            ImpactAssessment Impact,
            Option<AutomatedResponse> SuggestedAction);

        public IObservable<PrioritizedAlert> PrioritizeAlerts(IObservable<Alert> alertStream) =>
            alertStream
                .Select(alert => new PrioritizedAlert(
                    BaseAlert: alert,
                    Priority: CalculatePriority(alert),
                    Impact: new ImpactAssessment(1000m, 500m),
                    SuggestedAction: Some(new AutomatedResponse("Review immediately", 0.9m))))
                .Scan(new List<PrioritizedAlert>(), (acc, alert) =>
                {
                    acc.Add(alert);
                    return acc.OrderByDescending(a => a.Priority).Take(100).ToList();
                })
                .SelectMany(list => list.Take(10));

        private decimal CalculatePriority(Alert alert) =>
            alert.Type == "Critical" ? 100m :
            alert.Type == "High" ? 75m :
            alert.Type == "Medium" ? 50m : 25m;
    }

    public class ComplianceMaturityAssessor
    {
        public record MaturityAssessment(
            MaturityLevel CurrentLevel,
            decimal MaturityScore,
            Dictionary<string, CapabilityScore> Capabilities,
            IEnumerable<MaturityGap> Gaps,
            RoadmapToNextLevel Roadmap);

        public Either<string, MaturityAssessment> AssessMaturity(
            MerchantDto merchant,
            IEnumerable<AssessmentDto> assessments)
        {
            try
            {
                var capabilities = new Dictionary<string, CapabilityScore>
                {
                    ["Compliance"] = new("Compliance", merchant.ComplianceRank),
                    ["Assessment"] = new("Assessment", assessments.Any() ? 80m : 20m),
                    ["Volume Management"] = new("Volume Management", merchant.MerchantLevel <= 2 ? 80m : 40m)
                };

                var score = capabilities.Values.Average(c => c.Score);
                var level = score switch
                {
                    >= 90 => MaturityLevel.Optimizing,
                    >= 75 => MaturityLevel.Quantified,
                    >= 60 => MaturityLevel.Defined,
                    >= 40 => MaturityLevel.Managed,
                    _ => MaturityLevel.Initial
                };

                var gaps = capabilities
                    .Where(c => c.Value.Score < 80)
                    .Select(c => new MaturityGap(c.Key, c.Value.Score, 80m));

                var roadmap = new RoadmapToNextLevel(
                    new List<string> { "Improve compliance", "Increase assessment frequency" },
                    TimeSpan.FromDays(90));

                return new MaturityAssessment(
                    CurrentLevel: level,
                    MaturityScore: score,
                    Capabilities: capabilities,
                    Gaps: gaps,
                    Roadmap: roadmap);
            }
            catch (Exception ex)
            {
                return Left($"Maturity assessment failed: {ex.Message}");
            }
        }
    }
    public class AutomatedRecommendation
    {
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Priority { get; set; } = "";
        public string EstimatedImpact { get; set; } = "";
        public string[] ActionItems { get; set; } = System.Array.Empty<string>();
    }

    public class MerchantAssessmentInsight
    {
        public Guid MerchantId { get; set; }
        public string MerchantName { get; set; } = "";
        public int DaysOverdue { get; set; }
        public decimal LastComplianceScore { get; set; }
        public string RiskLevel { get; set; } = "";
    }

}